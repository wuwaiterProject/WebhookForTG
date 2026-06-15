#!/usr/bin/env python3
import json
import os
import sys
import urllib.request


def escape_html(text: str) -> str:
    return (
        text.replace("&", "&amp;")
        .replace("<", "&lt;")
        .replace(">", "&gt;")
    )


def main() -> None:
    token = os.environ["TELEGRAM_BOT_TOKEN"]
    chat_id = os.environ["TELEGRAM_CHAT_ID"]

    commit_msg, author, branch, short_hash, repo = sys.argv[1:6]

    text = (
        "<b>Git Push</b>\n"
        f"Repo  : <code>{escape_html(repo)}</code>\n"
        f"Branch: <code>{escape_html(branch)}</code>\n"
        f"Commit: <code>{escape_html(short_hash)}</code>\n"
        f"Author: {escape_html(author)}\n\n"
        f"{escape_html(commit_msg)}"
    )

    payload = json.dumps(
        {
            "chat_id": chat_id,
            "text": text,
            "parse_mode": "HTML",
        }
    ).encode("utf-8")

    req = urllib.request.Request(
        f"https://api.telegram.org/bot{token}/sendMessage",
        data=payload,
        headers={"Content-Type": "application/json; charset=utf-8"},
        method="POST",
    )

    with urllib.request.urlopen(req) as response:
        body = response.read().decode("utf-8")
        print(body)


if __name__ == "__main__":
    main()
