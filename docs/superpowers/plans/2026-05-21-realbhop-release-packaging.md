# RealBhop CS2 Lean Release Packaging Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship a smaller GitHub release zip by packaging only the plugin assembly and required runtime metadata.

**Architecture:** Keep the existing `dotnet publish` step to compile the plugin, then copy an allowlisted set of files into a dedicated package folder and zip that folder. This avoids bundling framework/shared DLLs already provided by CounterStrikeSharp.

**Tech Stack:** .NET 8, GitHub Actions, CounterStrikeSharp plugin runtime.

---

## File Structure
- Modify: `.github/workflows/build.yml` — adjust publish output and package only allowlisted files.
- (No new source files.)

---

### Task 1: Publish to a staging folder

**Files:**
- Modify: `.github/workflows/build.yml`

- [ ] **Step 1: Update publish output folder**

Change the publish step to emit into a dedicated subfolder so we can package from a clean staging directory.

```yaml
- name: Publish plugin
  run: dotnet publish src/RealBhopCS2/RealBhopCS2.csproj --configuration Release --no-build --output artifacts/RealBhopCS2/publish
```

- [ ] **Step 2: Verify publish output**

Run locally if needed:

```
dotnet publish src/RealBhopCS2/RealBhopCS2.csproj --configuration Release --output artifacts/RealBhopCS2/publish
```

Expected: `artifacts/RealBhopCS2/publish/RealBhopCS2.dll` exists.

- [ ] **Step 3: Commit**

```bash
git add .github/workflows/build.yml
git commit -m "chore: publish to staging folder"
```

---

### Task 2: Package only the plugin allowlist

**Files:**
- Modify: `.github/workflows/build.yml`

- [ ] **Step 1: Add a packaging step that copies only allowlisted files**

Insert a new step after publish:

```yaml
- name: Package plugin (lean)
  run: |
    rm -rf artifacts/RealBhopCS2/package
    mkdir -p artifacts/RealBhopCS2/package
    cp artifacts/RealBhopCS2/publish/RealBhopCS2.dll artifacts/RealBhopCS2/package/
    if [ -f artifacts/RealBhopCS2/publish/RealBhopCS2.deps.json ]; then cp artifacts/RealBhopCS2/publish/RealBhopCS2.deps.json artifacts/RealBhopCS2/package/; fi
    if [ -f artifacts/RealBhopCS2/publish/RealBhopCS2.runtimeconfig.json ]; then cp artifacts/RealBhopCS2/publish/RealBhopCS2.runtimeconfig.json artifacts/RealBhopCS2/package/; fi
```

- [ ] **Step 2: Update the zip step to use the package folder**

```yaml
- name: Package plugin
  run: |
    cd artifacts
    zip -r RealBhopCS2-${{ github.sha }}.zip RealBhopCS2/package
```

- [ ] **Step 3: Verify zip contents**

Run locally if needed:

```
zipinfo -1 artifacts/RealBhopCS2-<sha>.zip
```

Expected: only `RealBhopCS2.dll` (and optional `*.deps.json`, `*.runtimeconfig.json` if present).

- [ ] **Step 4: Commit**

```bash
git add .github/workflows/build.yml
git commit -m "chore: package lean release artifact"
```

---

### Task 3: Validate runtime on a clean server

**Files:**
- No code changes.

- [ ] **Step 1: Install the new release zip**

Copy the extracted folder into:

```
addons/counterstrikesharp/plugins/RealBhopCS2
```

- [ ] **Step 2: Verify plugin loads**

Expected: server console logs `RealBhop CS2 <version> loaded` and `css_realbhop_status` works.

---

## Plan Self-Review
- **Spec coverage:** Packaging is lean and allowlisted; validation steps included.
- **Placeholder scan:** No TODO/TBD placeholders.
- **Type consistency:** Only workflow changes and shell commands; consistent paths.
