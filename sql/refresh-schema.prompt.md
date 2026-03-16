# Refresh Schema Snapshot

Run the schema query against the local SocialMotive database and update the schema snapshot in `sql/claude.md` with the results.

## Steps

1. Run this command in the terminal:
   ```
   sqlcmd -S . -d SocialMotive -E -i sql\query-schema.sql -W
   ```

2. Read the output and update the **Schema Snapshot** section in `sql/claude.md`:
   - Update the **Last updated** date
   - Replace the **Tables & Columns** code block with current output
   - Replace the **Row Counts** code block with current counts

3. Do NOT change any other sections in `sql/claude.md`.
