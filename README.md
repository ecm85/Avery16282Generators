# Avery16282Generators

This repository has code for generating labels to print onto Avery 16282 labels.

The repo has labels for:

- Aeon's End
- Brewcrafters
- Dominion
- Legendary
- Spirit Island

## Dominion

This generator uses json and images sourced from Sumpfork's repo, with the following deviations:

 - Curse - color-shifted Victory instead of the one-off curse image
 - Debt - fixed the transparency issue

## AH LCG

This generator uses images from:
https://drive.google.com/file/d/1DEAMnd8HWTrFD9uFMquv_DjeaJ8WyYsp/view
Originally linked from:
https://boardgamegeek.com/thread/1671881/article/41274386#41274386
Assumptions:
Each folder (other than "Investigator Starter Decks", "Parallel Challenge Scenarios and Books", "Standalone Scenarios") follows this format:
1. Folder name: scenario name
2. Directly in folder:
    _<Scenario Name>.png
    Encounter sets...png (note: must not have " - ") in their names
3. Subfolder: Scenarios...
    <Scenario abbereviation> <Scenario #> - <Scenario name>



