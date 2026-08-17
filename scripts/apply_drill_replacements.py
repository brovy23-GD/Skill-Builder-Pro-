#!/usr/bin/env python3
"""Apply the reviewed 21-video correction and validate the complete 900-row dataset."""

from __future__ import annotations

import json
import os
import re
import tempfile
import urllib.parse
import urllib.request
from collections import Counter
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parent
SUPPORTED_SPORTS = {"BASEBALL", "SOFTBALL", "BASKETBALL", "SOCCER", "FOOTBALL", "HOCKEY"}


def fetch(url: str) -> bytes:
    request = urllib.request.Request(url, headers={"User-Agent": "Mozilla/5.0 (SkillBuilderProValidator/1.0)"})
    with urllib.request.urlopen(request, timeout=15) as response:
        if response.status != 200:
            raise RuntimeError(f"HTTP {response.status} validating a YouTube resource")
        return response.read()


def metadata(video_id: str) -> tuple[dict, int]:
    query = urllib.parse.urlencode({"url": f"https://www.youtube.com/watch?v={video_id}", "format": "json"})
    details = json.loads(fetch(f"https://www.youtube.com/oembed?{query}"))
    watch = fetch(f"https://www.youtube.com/watch?v={video_id}").decode("utf-8", errors="replace")
    match = re.search(r'"lengthSeconds":"(\d+)"', watch)
    seconds = int(match.group(1)) if match else 0
    return details, seconds


def display_duration(seconds: int) -> str:
    if seconds <= 0:
        return ""
    hours, remainder = divmod(seconds, 3600)
    minutes, seconds = divmod(remainder, 60)
    return f"{hours}:{minutes:02d}:{seconds:02d}" if hours else f"{minutes}:{seconds:02d}"


def atomic_json(path: Path, value: object) -> None:
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


def main() -> int:
    seed_path = ROOT / "drills_seed.external.json"
    audit_path = ROOT / "drills_audit.external.json"
    records = json.loads(seed_path.read_text(encoding="utf-8"))
    audit = json.loads(audit_path.read_text(encoding="utf-8"))
    replacements = json.loads((ROOT / "replacement_candidates.json").read_text(encoding="utf-8"))
    if len(replacements) != 21:
        raise RuntimeError("Exactly 21 reviewed replacements are required.")

    audit_groups = {
        (item["sport"], item["category"], item["subCategory"]): item
        for item in audit["results"]
    }
    verified_at = datetime.now(timezone.utc).isoformat(timespec="seconds")
    for record in records:
        replacement = replacements.get(str(record["id"]))
        if not replacement:
            continue
        details, seconds = metadata(replacement["videoId"])
        if details.get("title") != replacement["title"]:
            raise RuntimeError(f"YouTube title changed during validation for row {record['id']}.")
        if seconds and not (30 <= seconds <= 1800):
            raise RuntimeError(f"Replacement row {record['id']} is outside the 30-second to 30-minute range.")

        difficulty = int(record["difficulty"])
        duration = display_duration(seconds) or record["duration"]
        channel = details.get("author_name", replacement["channel"])
        record.update({
            "name": details["title"],
            "description": (
                f"A {record['sport'].lower()} {record['subCategory'].lower()} training video selected from {channel}. "
                f"Ranked {difficulty} of 5 for this subcategory using availability and sport-specific relevance."
            ),
            "duration": duration,
            "videoUrl": f"https://www.youtube.com/watch?v={replacement['videoId']}",
        })
        group = audit_groups[(record["sport"], record["category"], record["subCategory"])]
        selected = group["selected"][difficulty - 1]
        selected.update({
            "videoId": replacement["videoId"],
            "title": record["name"],
            "channel": channel,
            "views": 0,
            "likes": 0,
            "duration": duration,
            "score": 0.0,
            "verification": "Live YouTube oEmbed and watch-page validation",
            "verifiedAtUtc": verified_at,
        })

    if len(records) != 900 or [item["id"] for item in records] != list(range(1, 901)):
        raise RuntimeError("Dataset must contain IDs 1 through 900 exactly once.")
    if {item["sport"] for item in records} != SUPPORTED_SPORTS:
        raise RuntimeError("Dataset sport set does not match the supported allowlist.")
    urls = [item["videoUrl"] for item in records]
    if len(set(urls)) != 900:
        raise RuntimeError("Dataset video URLs are not unique.")
    groups = Counter((item["sport"], item["category"], item["subCategory"]) for item in records)
    if len(groups) != 180 or set(groups.values()) != {5}:
        raise RuntimeError("Dataset must contain exactly five videos in each of 180 subcategories.")
    if any(not re.fullmatch(r"https://www\.youtube\.com/watch\?v=[A-Za-z0-9_-]{11}", url) for url in urls):
        raise RuntimeError("A dataset URL is not a canonical YouTube watch URL.")

    audit.update({
        "generatedAt": datetime.now().isoformat(timespec="seconds"),
        "recordCount": 900,
        "uniqueVideoUrlCount": 900,
        "subCategoryCount": 180,
        "videosPerSubCategory": 5,
        "correctionCount": 21,
        "correctionVerifiedAtUtc": verified_at,
    })
    atomic_json(seed_path, records)
    atomic_json(audit_path, audit)
    print("CORRECTION_APPLIED=21")
    print("RECORDS=900 UNIQUE_URLS=900 SUBCATEGORIES=180 VIDEOS_PER_SUBCATEGORY=5")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
