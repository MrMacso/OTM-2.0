# Article: 1944

Article: 1944 is a first-person survival horror game set in an alternative museum inspired by Budapest's House of Terror. A reporter sneaks into the sealed museum after a mysterious death and discovers an antique pocket watch that links his heartbeat to time travel.

## Core Concept

- Genre: survival horror
- Theme: the duality of 20th-century Hungarian history, Arrow Cross rule, and a present-day investigation
- Mood: suffocating, tense, dark, and morally heavy
- Core gameplay: unarmed stealth, investigation, sabotage, and puzzle solving between timelines
- Location: a museum-like building that shifts between the present and 1944

## Player Motivation

The first time travel event changes the present and places the reporter's own name on the Wall of Victims. The pocket watch becomes linked to his pulse, making escape impossible without resolving the past. The murdered security guard is personally connected to the reporter, and the investigation confirms the disappearances he has pursued for years.

## Core Mechanics

### Pulse and Watch System

The watch replaces a traditional stamina timer. Calm stealth keeps the pulse low and lets the player remain in the past longer. Panic, chases, and witnessing traumatic events spike the pulse and make the watch drain rapidly. At a critical pulse level, the reporter is forced back to the present or dies, depending on final design.

### Butterfly Effect

Actions in 1944 change the museum in the present. A key stolen in the past can open a cassette box in the present. A wall visible in the modern floor plan may not exist yet in 1944, revealing a hidden route. Boss eliminations and sabotage gradually remove saved victims' names from the Wall of Victims.

### Time Travel

Time travel should change room layouts, reveal clues, alter enemy behavior, and enable item-based puzzle solutions.

## Current Demo Milestone

Build a stable playable demo with no known blocking bugs. The first target slice is:

1. Explore the present-day museum.
2. Find or inspect the pocket watch.
3. Trigger the first time travel event.
4. Use past/present differences to solve an early puzzle.
5. Return to the present and see a clear consequence.

## Implementation Direction

The current project foundation is organized around these systems:

- Player movement and control modes
- Interactable objects and prompts
- Inventory and key items
- Progress flags and stage definitions
- Timeline object activation
- Guidebook/investigation UI
- Horror events and monster AI

Keep new work focused on the playable slice before expanding the full story structure.
