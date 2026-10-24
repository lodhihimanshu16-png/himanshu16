// Claude Code PostToolUse hook: after a successful `git commit` run via Bash, refresh CLAUDE.md.
const { execFileSync, spawnSync } = require("child_process");
const path = require("path");

let input = "";
process.stdin.on("data", (d) => (input += d));
process.stdin.on("end", () => {
  let event;
  try {
    event = JSON.parse(input);
  } catch {
    return;
  }
  const cmd = event.tool_input && event.tool_input.command;
  if (!cmd || !/\bgit\s+commit\b/.test(cmd)) return;
  const resp = event.tool_response || {};
  if (resp.interrupted || resp.is_error) return;

  // The git post-commit hook has already done the work when it is enabled.
  try {
    const hooksPath = execFileSync("git", ["config", "core.hooksPath"], { encoding: "utf8" }).trim();
    if (hooksPath === ".githooks") return;
  } catch {
    // core.hooksPath not set
  }
  spawnSync("node", [path.join(__dirname, "update-claude-md.js")], { stdio: "inherit" });
});
