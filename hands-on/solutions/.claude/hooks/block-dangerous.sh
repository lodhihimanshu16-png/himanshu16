#!/usr/bin/env bash
# PreToolUse hook for Bash. Stops dangerous commands before they run.
# Claude Code sends JSON on stdin. The command is in tool_input.command.
# Exit 0 = allow. Exit 2 = block, and the text on stderr goes back to Claude.

input=$(cat)

# Pull out the command text. (No jq needed.) Escaped quotes inside the command are handled.
cmd=$(printf '%s' "$input" | sed -En 's/.*"command"[[:space:]]*:[[:space:]]*"(([^"\\]|\\.)*)".*/\1/p' | head -n 1)

block() {
  echo "Blocked by .claude/hooks/block-dangerous.sh: $1" 1>&2
  echo "Do not try to get around this. Pick a safer way, or ask the user to run it themselves." 1>&2
  exit 2
}

# rm with both a recursive flag and a force flag (-rf, -fr, -Rf, -rfv ...)
if [[ "$cmd" =~ (^|[^[:alnum:]_-])rm[[:space:]] ]] &&
   [[ "$cmd" =~ [[:space:]]-[a-zA-Z]*[rR][a-zA-Z]*[fF]|[[:space:]]-[a-zA-Z]*[fF][a-zA-Z]*[rR]|--recursive.*--force|--force.*--recursive|[[:space:]]-[rR]([[:space:]].*)?[[:space:]]-[fF]|[[:space:]]-[fF]([[:space:]].*)?[[:space:]]-[rR] ]]; then
  block "rm -rf can delete a lot by mistake. Delete specific files, or use 'dotnet clean' for bin and obj."
fi

# git push --force or -f
if [[ "$cmd" =~ git[[:space:]]+push[[:space:]] ]] &&
   [[ "$cmd" =~ [[:space:]]--force|[[:space:]]-[a-zA-Z]*f[a-zA-Z]*([[:space:]]|$) ]]; then
  block "a force push can erase other people's work. Use a normal 'git push'."
fi

# git reset --hard
if [[ "$cmd" =~ git[[:space:]]+reset[[:space:]].*--hard ]]; then
  block "'git reset --hard' throws away uncommitted work. Use 'git stash' or 'git restore <file>'."
fi

# git clean -f, -fd, -xdf, --force
if [[ "$cmd" =~ git[[:space:]]+clean[[:space:]] ]] &&
   [[ "$cmd" =~ [[:space:]]-[a-zA-Z]*f|--force ]]; then
  block "'git clean -f' deletes files that git does not track, for good."
fi

# npm publish
if [[ "$cmd" =~ npm[[:space:]]+publish ]]; then
  block "'npm publish' puts code on the public internet. The user must do this by hand."
fi

# curl or wget piped into a shell
if [[ "$cmd" =~ (curl|wget)[^|]*\|[[:space:]]*(sudo[[:space:]]+)?(sh|bash|zsh)([[:space:]]|$) ]]; then
  block "running a downloaded script without reading it is unsafe. Download it first and read it."
fi

# deleting appsettings files
if [[ "$cmd" =~ (rm|unlink|del)[[:space:]].*appsettings ]]; then
  block "appsettings files hold the app's settings. Do not delete them."
fi

exit 0
