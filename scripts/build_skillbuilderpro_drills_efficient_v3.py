#!/usr/bin/env python3
"""
Build Skill Builder Pro's 900-record drill library using the YouTube Data API v3.

Usage:
    set YOUTUBE_API_KEY=your_key_here          # Windows Command Prompt
    $env:YOUTUBE_API_KEY="your_key_here"       # PowerShell
    export YOUTUBE_API_KEY="your_key_here"     # macOS/Linux
    python build_skillbuilderpro_drills.py

Output:
    SkillBuilderPro_900_Drills.json
    SkillBuilderPro_900_Drills_Audit.json
"""

from __future__ import annotations

import json
import math
import os
import re
import sys
import tempfile
import time
from datetime import datetime
from pathlib import Path
from typing import Any

import requests

API_ROOT = "https://www.googleapis.com/youtube/v3"
CREATED_DATE = "2026-08-06T00:00:00"
CANDIDATES_PER_QUERY = 50
VIDEOS_PER_SUBCATEGORY = 5
REQUEST_TIMEOUT = 30
REQUEST_DELAY_SECONDS = 0.15

ROOT = Path(__file__).resolve().parent
HIERARCHY_FILE = ROOT / "SkillBuilderPro_Drill_Hierarchy.json"
OUTPUT_FILE = ROOT / "drills_seed.json"
CHECKPOINT_FILE = ROOT / "drills_seed_checkpoint.json"
SEARCH_CACHE_FILE = ROOT / "youtube_search_cache.json"
AUDIT_FILE = ROOT / "SkillBuilderPro_900_Drills_Audit.json"

PREFERRED_CHANNEL_TERMS = (
    "little league", "mlb", "usa baseball", "usab", "ripken", "img academy",
    "ultimate baseball training", "antonelli", "fastpitch power", "usa softball",
    "breakthrough basketball", "shotmechanics", "by any means basketball",
    "ilovebasketballtv", "fiba", "pga", "7mlc", "all attack", "unisport",
    "progressive soccer", "nfl", "usa football", "first down training",
    "quarterback academy", "hockey training", "coach jeremy", "how to hockey",
    "ice hockey systems", "hockey canada"
)

NEGATIVE_TITLE_TERMS = (
    "shorts", "#shorts", "reaction", "highlights", "compilation", "gameplay",
    "video game", "madden", "nhl 2k", "nba 2k", "fails", "funny moments"
)

SUPPORTED_SPORTS = {"BASEBALL", "SOFTBALL", "BASKETBALL", "SOCCER", "FOOTBALL", "HOCKEY"}
SPORT_TERMS = {
    "BASEBALL": ("baseball", "mlb", "infield", "catcher"),
    "SOFTBALL": ("softball", "fastpitch"),
    "BASKETBALL": ("basketball", "nba"),
    "SOCCER": ("soccer", "football", "futbol"),
    "FOOTBALL": ("football", "nfl", "linebacker", "quarterback", "wide receiver"),
    "HOCKEY": ("hockey", "nhl", "goalie"),
}
INCOMPATIBLE_SPORT_TERMS = {
    "BASEBALL": ("cricket", "golf", "tennis", "volleyball"),
    "SOFTBALL": ("cricket", "golf", "tennis", "volleyball"),
    "BASKETBALL": ("volleyball", "cricket", "soccer", "rugby"),
    "SOCCER": ("rugby", "nfl", "american football"),
    "FOOTBALL": ("rugby", "soccer", "fifa", "futbol"),
    "HOCKEY": ("field hockey", "air hockey", "video game"),
}


def api_get(endpoint: str, params: dict[str, Any], api_key: str) -> dict[str, Any]:
    params = {**params, "key": api_key}
    response = requests.get(
        f"{API_ROOT}/{endpoint}",
        params=params,
        timeout=REQUEST_TIMEOUT,
    )
    try:
        payload = response.json()
    except ValueError as exc:
        raise RuntimeError(f"Non-JSON response from YouTube: {response.status_code}") from exc

    if not response.ok:
        message = payload.get("error", {}).get("message", response.text)
        if response.status_code == 429:
            raise RuntimeError(
                "YouTube daily search quota has been reached. "
                "Your checkpoint and search cache are preserved. "
                "Run this same script again after the quota resets. "
                f"Original error: {message}"
            )
        raise RuntimeError(f"YouTube API error {response.status_code}: {message}")

    time.sleep(REQUEST_DELAY_SECONDS)
    return payload


def parse_iso8601_duration(value: str) -> str:
    match = re.fullmatch(
        r"P(?:(?P<days>\d+)D)?T(?:(?P<hours>\d+)H)?(?:(?P<minutes>\d+)M)?(?:(?P<seconds>\d+)S)?",
        value or "",
    )
    if not match:
        return ""

    days = int(match.group("days") or 0)
    hours = int(match.group("hours") or 0) + days * 24
    minutes = int(match.group("minutes") or 0)
    seconds = int(match.group("seconds") or 0)

    if hours:
        return f"{hours}:{minutes:02d}:{seconds:02d}"
    return f"{minutes}:{seconds:02d}"


def normalize(text: str) -> str:
    return re.sub(r"[^a-z0-9]+", " ", text.lower()).strip()


def contains_term(text: str, term: str) -> bool:
    return re.search(rf"(?<![a-z0-9]){re.escape(normalize(term))}(?![a-z0-9])", text) is not None


def video_matches_context(video: dict[str, Any], sport: str, category: str, subcategory: str) -> bool:
    if sport not in SUPPORTED_SPORTS:
        return False
    snippet = video.get("snippet", {})
    combined = normalize(
        f"{snippet.get('title', '')} {snippet.get('description', '')} {snippet.get('channelTitle', '')}"
    )
    if any(contains_term(combined, term) for term in INCOMPATIBLE_SPORT_TERMS[sport]):
        return False
    if not any(contains_term(combined, term) for term in SPORT_TERMS[sport]):
        return False
    skill_tokens = [token for token in normalize(f"{category} {subcategory}").split() if len(token) >= 4]
    return not skill_tokens or any(contains_term(combined, token) for token in skill_tokens)


def atomic_write_json(path: Path, value: Any) -> None:
    fd, temp_name = tempfile.mkstemp(prefix=f".{path.name}.", suffix=".tmp", dir=path.parent)
    try:
        with os.fdopen(fd, "w", encoding="utf-8", newline="\n") as stream:
            json.dump(value, stream, indent=2, ensure_ascii=False)
            stream.write("\n")
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temp_name, path)
    except Exception:
        try:
            os.unlink(temp_name)
        except OSError:
            pass
        raise


def query_for(sport: str, category: str, subcategory: str) -> str:
    if category == "Workout":
        return f"{sport} {subcategory} workout training exercises"
    return f"{sport} {category} {subcategory} drills coaching tutorial"


def keyword_relevance(title: str, description: str, sport: str, category: str, subcategory: str) -> float:
    haystack = normalize(f"{title} {description}")
    terms = set(normalize(f"{sport} {category} {subcategory} drill training workout").split())
    matched = sum(1 for term in terms if term in haystack)
    return matched / max(len(terms), 1)


def candidate_score(video: dict[str, Any], sport: str, category: str, subcategory: str) -> float:
    title = video["snippet"].get("title", "")
    description = video["snippet"].get("description", "")
    channel = video["snippet"].get("channelTitle", "")
    stats = video.get("statistics", {})

    views = int(stats.get("viewCount", 0))
    likes = int(stats.get("likeCount", 0))
    relevance = keyword_relevance(title, description, sport, category, subcategory)

    score = math.log10(views + 1) * 4.0
    score += math.log10(likes + 1) * 1.5
    score += relevance * 8.0

    channel_norm = normalize(channel)
    if any(term in channel_norm for term in PREFERRED_CHANNEL_TERMS):
        score += 3.0

    title_norm = normalize(title)
    if "drill" in title_norm or "workout" in title_norm or "training" in title_norm:
        score += 1.5
    if any(term in title.lower() for term in NEGATIVE_TITLE_TERMS):
        score -= 8.0

    duration_seconds = iso_duration_seconds(video.get("contentDetails", {}).get("duration", ""))
    if 60 <= duration_seconds <= 1800:
        score += 1.0
    elif duration_seconds < 30:
        score -= 4.0

    return score


def iso_duration_seconds(value: str) -> int:
    match = re.fullmatch(
        r"P(?:(?P<days>\d+)D)?T(?:(?P<hours>\d+)H)?(?:(?P<minutes>\d+)M)?(?:(?P<seconds>\d+)S)?",
        value or "",
    )
    if not match:
        return 0
    return (
        int(match.group("days") or 0) * 86400
        + int(match.group("hours") or 0) * 3600
        + int(match.group("minutes") or 0) * 60
        + int(match.group("seconds") or 0)
    )


def search_candidates(api_key: str, query: str) -> list[str]:
    payload = api_get(
        "search",
        {
            "part": "snippet",
            "q": query,
            "type": "video",
            "maxResults": CANDIDATES_PER_QUERY,
            "order": "viewCount",
            "regionCode": "US",
            "safeSearch": "moderate",
            "videoEmbeddable": "true",
            "videoSyndicated": "true",
        },
        api_key,
    )
    return [
        item["id"]["videoId"]
        for item in payload.get("items", [])
        if item.get("id", {}).get("videoId")
    ]


def fetch_video_details(api_key: str, video_ids: list[str]) -> list[dict[str, Any]]:
    if not video_ids:
        return []

    payload = api_get(
        "videos",
        {
            "part": "snippet,contentDetails,statistics,status",
            "id": ",".join(video_ids),
        },
        api_key,
    )

    results = []
    for item in payload.get("items", []):
        status = item.get("status", {})
        if status.get("privacyStatus") != "public":
            continue
        if status.get("embeddable") is False:
            continue
        results.append(item)
    return results


def make_description(
    sport: str,
    category: str,
    subcategory: str,
    channel: str,
    rank: int,
) -> str:
    skill = subcategory.lower()
    if category == "Workout":
        return (
            f"A {sport.lower()}-specific {skill} workout selected from {channel}. "
            f"Ranked {rank} of 5 for this subcategory using popularity, engagement, "
            f"relevance, instruction quality, and video availability."
        )
    return (
        f"A {sport.lower()} {skill} training video selected from {channel}. "
        f"Ranked {rank} of 5 for this subcategory using popularity, engagement, "
        f"relevance, instruction quality, and video availability."
    )



def load_search_cache() -> dict[str, list[str]]:
    if not SEARCH_CACHE_FILE.exists():
        return {}
    try:
        data = json.loads(SEARCH_CACHE_FILE.read_text(encoding="utf-8"))
        if not isinstance(data, dict):
            return {}
        return {
            query: ids for query, ids in data.items()
            if isinstance(query, str)
            and isinstance(ids, list)
            and all(isinstance(item, str) and re.fullmatch(r"[A-Za-z0-9_-]{11}", item) for item in ids)
        }
    except (json.JSONDecodeError, OSError):
        return {}


def save_search_cache(cache: dict[str, list[str]]) -> None:
    atomic_write_json(SEARCH_CACHE_FILE, cache)


def cached_search_candidates(
    api_key: str,
    query: str,
    cache: dict[str, list[str]],
) -> list[str]:
    if query in cache:
        print("  Using cached YouTube search results")
        return cache[query]

    ids = search_candidates(api_key, query)
    cache[query] = ids
    save_search_cache(cache)
    return ids


def targeted_fallback_queries(
    sport: str,
    category: str,
    subcategory: str,
) -> list[str]:
    """Extra search phrases for labels that YouTube creators name differently."""
    key = (sport.upper(), category.lower(), subcategory.lower())

    aliases = {
        ("SOFTBALL", "baserunning", "quick starts"): [
            "softball baserunning first step quickness drills",
            "softball home to first speed drills",
            "softball baserunning acceleration starts",
            "softball sprint start baserunning drills",
            "fastpitch softball first step baserunning drills",
        ],
        ("BASEBALL", "baserunning", "first-step quickness"): [
            "baseball baserunning first step quickness drills",
            "baseball home to first speed drills",
            "baseball baserunning acceleration starts",
        ],
        ("BASEBALL", "baserunning", "lead-offs"): [
            "baseball lead off baserunning drills",
            "baseball primary secondary lead drills",
        ],
        ("SOFTBALL", "baserunning", "lead-offs"): [
            "softball lead off baserunning drills",
            "fastpitch softball lead off drills",
        ],
        ("FOOTBALL", "route running", "break points"): [
            "football route break drills",
            "wide receiver break point route running drills",
        ],
        ("FOOTBALL", "catching", "sideline control"): [
            "football sideline catch drills",
            "wide receiver toe tap catch drills",
            "football boundary catch drills",
            "wide receiver body control sideline catches",
            "football two feet in bounds catch drills",
            "receiver toe drag sideline drill",
        ],
        ("HOCKEY", "goalie", "stick saves"): [
            "hockey goalie stick save drills",
            "goalie paddle save stick drills hockey",
        ],
        ("SOCCER", "goalkeeper", "rebound control"): [
            "soccer goalkeeper rebound control drills",
            "goalkeeper parrying rebound drills",
        ],
    }

    return aliases.get(key, [])

def main() -> int:
    api_key = os.getenv("YOUTUBE_API_KEY", "").strip()
    if not api_key:
        print("ERROR: Set the YOUTUBE_API_KEY environment variable first.", file=sys.stderr)
        return 2

    if not HIERARCHY_FILE.exists():
        print(f"ERROR: Missing {HIERARCHY_FILE.name}", file=sys.stderr)
        return 2

    hierarchy = json.loads(HIERARCHY_FILE.read_text(encoding="utf-8"))
    if not isinstance(hierarchy, dict) or set(hierarchy) != SUPPORTED_SPORTS:
        raise RuntimeError("Hierarchy sports do not exactly match the supported sport allowlist.")
    search_cache = load_search_cache()
    print(f"Loaded {len(search_cache)} cached YouTube searches.")

    records: list[dict[str, Any]] = []
    used_video_ids: set[str] = set()
    audit: list[dict[str, Any]] = []
    completed_keys: set[str] = set()
    record_id = 1

    if CHECKPOINT_FILE.exists():
        checkpoint = json.loads(CHECKPOINT_FILE.read_text(encoding="utf-8"))
        if not isinstance(checkpoint, dict):
            raise RuntimeError("Checkpoint root must be an object.")
        records = checkpoint.get("records", [])
        audit = checkpoint.get("audit", [])
        raw_completed_keys = checkpoint.get("completedKeys", [])
        if not isinstance(records, list) or not isinstance(audit, list) or not isinstance(raw_completed_keys, list):
            raise RuntimeError("Checkpoint records, audit, and completedKeys must be arrays.")
        if [record.get("id") for record in records] != list(range(1, len(records) + 1)):
            raise RuntimeError("Checkpoint record IDs are not contiguous from 1.")
        checkpoint_urls = [record.get("videoUrl") for record in records]
        if any(not isinstance(url, str) for url in checkpoint_urls) or len(checkpoint_urls) != len(set(checkpoint_urls)):
            raise RuntimeError("Checkpoint contains missing or duplicate video URLs.")
        expected_record_count = len(raw_completed_keys) * VIDEOS_PER_SUBCATEGORY
        if len(records) != expected_record_count or len(audit) != len(raw_completed_keys):
            raise RuntimeError("Checkpoint counts are inconsistent; refusing an unsafe resume.")
        used_video_ids = {
            record["videoUrl"].split("v=", 1)[-1]
            for record in records
            if record.get("videoUrl")
        }
        completed_keys = set(raw_completed_keys)
        if len(completed_keys) != len(raw_completed_keys):
            raise RuntimeError("Checkpoint contains duplicate completedKeys.")
        record_id = len(records) + 1
        print(
            f"Resuming checkpoint: {len(completed_keys)} subcategories, "
            f"{len(records)} records already saved."
        )

    total_subcategories = sum(
        len(subcategories)
        for categories in hierarchy.values()
        for subcategories in categories.values()
    )
    completed = 0

    for sport, categories in hierarchy.items():
        for category, subcategories in categories.items():
            for subcategory in subcategories:
                completed += 1
                subcategory_key = f"{sport}|{category}|{subcategory}"
                if subcategory_key in completed_keys:
                    print(
                        f"[{completed}/{total_subcategories}] SKIP completed: "
                        f"{sport} / {category} / {subcategory}"
                    )
                    continue

                query = query_for(sport, category, subcategory)
                print(f"[{completed}/{total_subcategories}] {sport} / {category} / {subcategory}")

                selected: list[dict[str, Any]] = []
                seen_for_query: set[str] = set()

                primary_query = query
                fallback_query = (
                    f"best {sport} {subcategory} drills training"
                    if category != "Workout"
                    else f"best {sport} {subcategory} workout exercises"
                )

                # Use one primary search and one normal fallback first.
                # For hard-to-match labels, append sport-specific synonym searches.
                query_plan = [
                    (primary_query, 0.10),
                    (fallback_query, 0.00),
                ]
                query_plan.extend(
                    (q, 0.0)
                    for q in targeted_fallback_queries(
                        sport, category, subcategory
                    )
                )

                for query_variant, threshold in query_plan:
                    ids = cached_search_candidates(
                        api_key,
                        query_variant,
                        search_cache,
                    )
                    details = fetch_video_details(api_key, ids)

                    for video in sorted(
                        details,
                        key=lambda item: candidate_score(
                            item, sport, category, subcategory
                        ),
                        reverse=True,
                    ):
                        video_id = video["id"]
                        title = video["snippet"].get("title", "").strip()
                        description = video["snippet"].get("description", "")

                        if video_id in used_video_ids or video_id in seen_for_query:
                            continue
                        if any(term in title.lower() for term in NEGATIVE_TITLE_TERMS):
                            continue
                        if not video_matches_context(video, sport, category, subcategory):
                            continue

                        relevance = keyword_relevance(
                            title,
                            description,
                            sport,
                            category,
                            subcategory,
                        )
                        if relevance < threshold:
                            continue

                        combined = normalize(f"{title} {description}")
                        sport_term = normalize(sport)
                        subcategory_term = normalize(subcategory)

                        # During the fallback pass, still require the sport or
                        # exact subcategory phrase so unrelated videos cannot slip in.
                        if threshold == 0.0:
                            skill_tokens = [
                                token for token in normalize(subcategory).split()
                                if len(token) >= 4
                            ]
                            sport_match = sport_term in combined
                            skill_match = (
                                subcategory_term in combined
                                or any(token in combined for token in skill_tokens)
                            )

                            synonym_match = False
                            if (
                                sport.upper() == "SOFTBALL"
                                and category.lower() == "baserunning"
                                and subcategory.lower() == "quick starts"
                            ):
                                synonym_match = any(
                                    phrase in combined
                                    for phrase in (
                                        "first step",
                                        "home to first",
                                        "acceleration",
                                        "sprint start",
                                        "baserunning start",
                                        "quickness",
                                    )
                                )

                            if (
                                sport.upper() == "FOOTBALL"
                                and category.lower() == "catching"
                                and subcategory.lower() == "sideline control"
                            ):
                                synonym_match = synonym_match or any(
                                    phrase in combined
                                    for phrase in (
                                        "sideline catch",
                                        "toe tap",
                                        "toe drag",
                                        "boundary catch",
                                        "body control",
                                        "in bounds",
                                        "two feet",
                                        "feet in",
                                    )
                                )

                            if not sport_match:
                                continue
                            if skill_tokens and not (skill_match or synonym_match):
                                continue

                        selected.append(video)
                        seen_for_query.add(video_id)

                        if len(selected) == VIDEOS_PER_SUBCATEGORY:
                            break

                    if len(selected) == VIDEOS_PER_SUBCATEGORY:
                        break

                if len(selected) < VIDEOS_PER_SUBCATEGORY:
                    rescue_query = (
                        f"{sport} {category} drills {subcategory}"
                        if category != "Workout"
                        else f"{sport} athlete workout {subcategory}"
                    )

                    ids = cached_search_candidates(
                        api_key,
                        rescue_query,
                        search_cache,
                    )
                    details = fetch_video_details(api_key, ids)

                    for video in sorted(
                        details,
                        key=lambda item: candidate_score(
                            item, sport, category, subcategory
                        ),
                        reverse=True,
                    ):
                        video_id = video["id"]
                        title = video["snippet"].get("title", "").strip()
                        description = video["snippet"].get("description", "")
                        combined = normalize(f"{title} {description}")

                        if video_id in used_video_ids or video_id in seen_for_query:
                            continue
                        if any(term in title.lower() for term in NEGATIVE_TITLE_TERMS):
                            continue
                        if not video_matches_context(video, sport, category, subcategory):
                            continue
                        if normalize(sport) not in combined:
                            continue

                        # For known alternate terminology, permit a synonym match.
                        rescue_synonym = False
                        if (
                            sport.upper() == "FOOTBALL"
                            and category.lower() == "catching"
                            and subcategory.lower() == "sideline control"
                        ):
                            rescue_synonym = any(
                                phrase in combined
                                for phrase in (
                                    "sideline",
                                    "toe tap",
                                    "toe drag",
                                    "boundary",
                                    "body control",
                                    "in bounds",
                                )
                            )

                        if not rescue_synonym:
                            skill_tokens = [
                                token for token in normalize(subcategory).split()
                                if len(token) >= 4
                            ]
                            if skill_tokens and not any(
                                token in combined for token in skill_tokens
                            ):
                                continue

                        selected.append(video)
                        seen_for_query.add(video_id)

                        if len(selected) == VIDEOS_PER_SUBCATEGORY:
                            break

                if len(selected) < VIDEOS_PER_SUBCATEGORY:
                    raise RuntimeError(
                        f"Only found {len(selected)} acceptable unique videos for "
                        f"{sport} / {category} / {subcategory}. "
                        f"The checkpoint and search cache were saved. "
                        f"No incomplete final JSON was written."
                    )

                selected.sort(
                    key=lambda item: candidate_score(item, sport, category, subcategory),
                    reverse=True,
                )

                audit_entry = {
                    "sport": sport,
                    "category": category,
                    "subCategory": subcategory,
                    "query": query,
                    "selected": [],
                }

                for difficulty, video in enumerate(selected, start=1):
                    video_id = video["id"]
                    snippet = video["snippet"]
                    stats = video.get("statistics", {})
                    duration = parse_iso8601_duration(
                        video.get("contentDetails", {}).get("duration", "")
                    )
                    channel = snippet.get("channelTitle", "YouTube creator")

                    record = {
                        "id": record_id,
                        "name": snippet.get("title", "").strip(),
                        "sport": sport,
                        "category": category,
                        "subCategory": subcategory,
                        "description": make_description(
                            sport, category, subcategory, channel, difficulty
                        ),
                        "difficulty": str(difficulty),
                        "duration": duration,
                        "videoUrl": f"https://www.youtube.com/watch?v={video_id}",
                        "dateCreated": CREATED_DATE,
                    }
                    records.append(record)
                    used_video_ids.add(video_id)
                    record_id += 1

                    audit_entry["selected"].append({
                        "videoId": video_id,
                        "title": record["name"],
                        "channel": channel,
                        "views": int(stats.get("viewCount", 0)),
                        "likes": int(stats.get("likeCount", 0)),
                        "duration": duration,
                        "score": round(
                            candidate_score(video, sport, category, subcategory), 3
                        ),
                    })

                audit.append(audit_entry)
                completed_keys.add(subcategory_key)

                atomic_write_json(
                    CHECKPOINT_FILE,
                    {
                        "records": records,
                        "audit": audit,
                        "completedKeys": sorted(completed_keys),
                    },
                )
                print(
                    f"  Saved checkpoint: {len(records)} of "
                    f"{total_subcategories * VIDEOS_PER_SUBCATEGORY} records"
                )

    expected = total_subcategories * VIDEOS_PER_SUBCATEGORY
    if len(records) != expected:
        raise RuntimeError(f"Expected {expected} records but created {len(records)}.")

    urls = [record["videoUrl"] for record in records]
    if len(urls) != len(set(urls)):
        raise RuntimeError("Duplicate videoUrl values detected.")

    atomic_write_json(OUTPUT_FILE, records)
    atomic_write_json(
        AUDIT_FILE,
        {
            "generatedAt": datetime.now().isoformat(timespec="seconds"),
            "recordCount": len(records),
            "uniqueVideoUrlCount": len(set(urls)),
            "subCategoryCount": total_subcategories,
            "videosPerSubCategory": VIDEOS_PER_SUBCATEGORY,
            "results": audit,
        },
    )

    if CHECKPOINT_FILE.exists():
        CHECKPOINT_FILE.unlink()

    print(f"\nCreated {OUTPUT_FILE.name}: {len(records)} records")
    print(f"Created {AUDIT_FILE.name}: rankings and source metrics")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
