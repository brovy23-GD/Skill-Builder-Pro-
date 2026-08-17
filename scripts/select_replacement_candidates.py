#!/usr/bin/env python3
"""Select live, sport-correct replacements for the 21 known contaminated drill rows."""

from __future__ import annotations

import json
import re
import time
import urllib.parse
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parent
BAD_IDS = {12, 44, 47, 49, 50, 101, 126, 178, 181, 191, 266, 273, 295, 362, 364, 375, 422, 592, 682, 692, 693}
REVIEWED_FALLBACKS = {693: "mlGPassF21k"}

SPORT_TERMS = {
    "BASEBALL": ("baseball", "mlb", "infield", "catcher"),
    "SOFTBALL": ("softball", "fastpitch"),
    "BASKETBALL": ("basketball", "nba"),
    "SOCCER": ("soccer", "football"),
    "FOOTBALL": ("football", "nfl", "linebacker", "defensive back"),
}
INCOMPATIBLE = {
    "BASEBALL": ("cricket", "golf", "tennis", "volleyball"),
    "SOFTBALL": ("cricket", "golf", "tennis", "volleyball"),
    "BASKETBALL": ("volleyball", "cricket", "soccer", "rugby"),
    "SOCCER": ("rugby", "nfl", "american football"),
    "FOOTBALL": ("rugby", "soccer", "fifa"),
}
SKILL_TERMS = {
    ("BASEBALL", "Fielding", "Ground Balls"): ("ground ball", "infield", "fielding"),
    ("BASEBALL", "Hitting", "Rotation — Power"): ("rotation", "rotational", "power", "hip"),
    ("BASEBALL", "Hitting", "Timing"): ("timing", "load", "stride", "hitting"),
    ("BASEBALL", "Catching", "Receiving"): ("receiving", "framing"),
    ("BASEBALL", "Workout", "Rotational Power"): ("rotational", "rotation", "power", "medicine ball"),
    ("SOFTBALL", "Hitting", "Bat Path"): ("bat path", "swing path", "on plane", "barrel"),
    ("SOFTBALL", "Hitting", "Timing"): ("timing", "load", "stride", "hitting"),
    ("SOFTBALL", "Hitting", "Power Rotation"): ("rotation", "rotational", "power", "hip"),
    ("SOFTBALL", "Catching", "Transfer Speed"): ("transfer", "pop time"),
    ("SOFTBALL", "Catching", "Footwork"): ("footwork", "blocking"),
    ("SOFTBALL", "Workout", "Lower-Body Strength"): ("lower body", "leg", "strength", "squat"),
    ("BASKETBALL", "Passing", "Overhead Pass"): ("overhead pass",),
    ("BASKETBALL", "Passing", "One-Hand Passing"): ("one hand", "one-handed"),
    ("BASKETBALL", "Footwork", "Balance"): ("balance",),
    ("SOCCER", "Workout", "Core Stability"): ("core", "stability", "balance"),
    ("FOOTBALL", "Tackling", "Angle Tackling"): ("angle tackle", "angle tackling", "tackling"),
    ("FOOTBALL", "Tackling", "Wrap-Up Technique"): ("wrap up", "wrap-up", "tackling", "tackle"),
}


def normalized(value: str) -> str:
    return re.sub(r"\s+", " ", value.lower()).strip()


def contains_term(text: str, term: str) -> bool:
    return re.search(rf"(?<![a-z0-9]){re.escape(term)}(?![a-z0-9])", text) is not None


def live_metadata(video_id: str) -> dict | None:
    query = urllib.parse.urlencode({"url": f"https://www.youtube.com/watch?v={video_id}", "format": "json"})
    request = urllib.request.Request(
        f"https://www.youtube.com/oembed?{query}",
        headers={"User-Agent": "SkillBuilderPro dataset validator/1.0"},
    )
    try:
        with urllib.request.urlopen(request, timeout=12) as response:
            if response.status != 200:
                return None
            return json.load(response)
    except Exception:
        return None


def live_duration_seconds(video_id: str) -> int:
    request = urllib.request.Request(
        f"https://www.youtube.com/watch?v={video_id}",
        headers={"User-Agent": "Mozilla/5.0 (compatible; SkillBuilderProValidator/1.0)"},
    )
    try:
        with urllib.request.urlopen(request, timeout=12) as response:
            html = response.read().decode("utf-8", errors="replace")
        match = re.search(r'"lengthSeconds":"(\d+)"', html)
        return int(match.group(1)) if match else 0
    except Exception:
        return 0


def live_search(query: str) -> list[str]:
    request = urllib.request.Request(
        "https://www.youtube.com/results?" + urllib.parse.urlencode({"search_query": query}),
        headers={"User-Agent": "Mozilla/5.0 (compatible; SkillBuilderProValidator/1.0)"},
    )
    try:
        with urllib.request.urlopen(request, timeout=15) as response:
            html = response.read().decode("utf-8", errors="replace")
    except Exception:
        return []
    # Search pages repeat IDs in navigation and rendering payloads. Preserving
    # first-seen order approximates YouTube's result order without requiring an API key.
    return list(dict.fromkeys(re.findall(r'"videoId":"([A-Za-z0-9_-]{11})"', html)))


def acceptable(record: dict, metadata: dict) -> bool:
    text = normalized(f"{metadata.get('title', '')} {metadata.get('author_name', '')}")
    sport = record["sport"]
    if any(contains_term(text, term) for term in INCOMPATIBLE[sport]):
        return False
    if "#shorts" in text or " shorts" in text or "youtubeshorts" in text:
        return False
    if not any(contains_term(text, term) for term in SPORT_TERMS[sport]):
        return False
    key = (sport, record["category"], record["subCategory"])
    if key == ("SOFTBALL", "Catching", "Footwork"):
        return "catcher" in text and "footwork" in text
    if key == ("SOFTBALL", "Hitting", "Power Rotation"):
        return "power" in text and ("rotation" in text or "rotational" in text)
    if key == ("FOOTBALL", "Tackling", "Wrap-Up Technique"):
        return (
            ("wrap" in text and ("tackle" in text or "tackling" in text))
            or "form tackle" in text
            or "form tackling" in text
        )
    skill_terms = SKILL_TERMS[key]
    return any(contains_term(text, term) for term in skill_terms)


def main() -> int:
    records = json.loads((ROOT / "drills_seed.external.json").read_text(encoding="utf-8"))
    cache = json.loads((ROOT / "youtube_search_cache.external.json").read_text(encoding="utf-8"))
    used = {item["videoUrl"].split("v=", 1)[-1] for item in records}
    selected: dict[int, dict] = {}

    for record in (item for item in records if item["id"] in BAD_IDS):
        needle = normalized(f"{record['sport']} {record['category']} {record['subCategory']}")
        terms = set(needle.split())
        queries = sorted(
            cache,
            key=lambda query: len(terms & set(normalized(query).split())),
            reverse=True,
        )
        replacement = None
        direct_query = f"{record['sport']} {record['category']} {record['subCategory']} training tutorial -shorts"
        for video_id in live_search(direct_query)[:40]:
            if video_id in used:
                continue
            metadata = live_metadata(video_id)
            time.sleep(0.04)
            if metadata and acceptable(record, metadata) and 30 <= live_duration_seconds(video_id) <= 1800:
                replacement = {
                    "videoId": video_id,
                    "title": metadata["title"],
                    "channel": metadata.get("author_name", "YouTube creator"),
                    "query": direct_query,
                }
                break
        for query in queries[:12]:
            if replacement:
                break
            if record["sport"].lower() not in query.lower():
                continue
            for video_id in cache[query]:
                if video_id in used:
                    continue
                metadata = live_metadata(video_id)
                time.sleep(0.04)
                if metadata and acceptable(record, metadata) and 30 <= live_duration_seconds(video_id) <= 1800:
                    replacement = {
                        "videoId": video_id,
                        "title": metadata["title"],
                        "channel": metadata.get("author_name", "YouTube creator"),
                        "query": query,
                    }
                    break
            if replacement:
                break
        if not replacement and record["id"] in REVIEWED_FALLBACKS:
            video_id = REVIEWED_FALLBACKS[record["id"]]
            candidate_metadata = live_metadata(video_id)
            if (
                video_id not in used
                and candidate_metadata
                and acceptable(record, candidate_metadata)
                and 30 <= live_duration_seconds(video_id) <= 1800
            ):
                replacement = {
                    "videoId": video_id,
                    "title": candidate_metadata["title"],
                    "channel": candidate_metadata.get("author_name", "YouTube creator"),
                    "query": "Reviewed sport-specific fallback",
                }
        if not replacement:
            print(f"MISSING {record['id']}: {record['sport']} / {record['category']} / {record['subCategory']}")
            continue
        selected[record["id"]] = replacement
        used.add(replacement["videoId"])
        print(f"{record['id']}: {replacement['title']} | {replacement['channel']} | {replacement['videoId']}")

    (ROOT / "replacement_candidates.json").write_text(
        json.dumps(selected, indent=2, ensure_ascii=False), encoding="utf-8"
    )
    print(f"SELECTED={len(selected)} EXPECTED={len(BAD_IDS)}")
    return 0 if len(selected) == len(BAD_IDS) else 1


if __name__ == "__main__":
    raise SystemExit(main())
