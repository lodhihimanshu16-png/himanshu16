// Regenerates the "Recent commits" block in CLAUDE.md and amends it into HEAD.
// Run by .githooks/post-commit and by the Claude Code PostToolUse hook.
const { execFileSync } = require("child_process");
const fs = require("fs");
const path = require("path");

if (process.env.CLAUDE_MD_HOOK === "1") process.exit(0);

const git = (...args) =>
  execFileSync("git", args, { encoding: "utf8", env: { ...process.env, CLAUDE_MD_HOOK: "1" } }).trim();

const root = git("rev-parse", "--show-toplevel");
const file = path.join(root, "CLAUDE.md");
if (!fs.existsSync(file)) process.exit(0);

const START = "<!-- AUTO:RECENT-COMMITS:START -->";
const END = "<!-- AUTO:RECENT-COMMITS:END -->";

// HEAD is listed by subject only: amending changes its hash, so a hash here would go stale.
const lines = git("log", "-10", "--pretty=format:%h\t%s")
  .split("\n")
  .map((l, i) => {
    const [hash, subject] = l.split("\t");
    return i === 0 ? `- (this commit) ${subject}` : `- ${hash} ${subject}`;
  });
const block = `${START}\n${lines.join("\n")}\n${END}`;

const original = fs.readFileSync(file, "utf8");
let updated;
if (original.includes(START) && original.includes(END)) {
  updated = original.replace(new RegExp(`${START}[\\s\\S]*?${END}`), () => block);
} else {
  updated = `${original.replace(/\s*$/, "")}\n\n## Recent commits\n\n${block}\n`;
}
if (updated === original) process.exit(0);

fs.writeFileSync(file, updated);
git("add", "CLAUDE.md");
git("commit", "--amend", "--no-edit", "--no-verify");
