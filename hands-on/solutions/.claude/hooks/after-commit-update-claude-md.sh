#!/usr/bin/env bash
# PostToolUse hook for Bash. After a git commit, asks Claude to keep CLAUDE.md up to date.
# Claude Code sends JSON on stdin. The command is in tool_input.command.
# Exit 0 = nothing to do. Exit 2 = send the text on stderr to Claude.

input=$(cat)

# Pull out the command text. (No jq needed.)
cmd=$(printf '%s' "$input" | sed -En 's/.*"command"[[:space:]]*:[[:space:]]*"(([^"\\]|\\.)*)".*/\1/p' | head -n 1)

# Not a commit: do nothing.
case "$cmd" in
  *"git commit"*) ;;
  *) exit 0 ;;
esac

# This is the CLAUDE.md commit itself: do nothing, so we do not loop forever.
case "$cmd" in
  *"docs: update CLAUDE.md"*) exit 0 ;;
esac

echo "You just made a commit. Run git show --stat HEAD, update CLAUDE.md (folder map, endpoints, rules) to match, then commit only CLAUDE.md with the message 'docs: update CLAUDE.md'." 1>&2
exit 2
