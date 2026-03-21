# BeachBum War Game

## Project Description

The project purpose is to develop the War card game for 2 players using Unity and C#.

---

## War Game Rules

- Standard 52-card pack. Cards rank high to low: **A K Q J T 9 8 7 6 5 4 3 2**. Suits are ignored.
- **Setup:** Shuffle and deal all cards — each player gets 26. Players do not look at their cards.
- **Objective:** Win all the cards.

### Normal Round
Both players flip their top card. The higher card wins both. When a player's deck runs out, they shuffle their side pile and use it as their deck.

### War (tied cards)
- Tied cards stay on the table.
- Both players place 3 cards face-down, then 1 card face-up.
- Higher face-up card wins all cards on the table (10 cards total).
- If face-up cards tie again, war continues: repeat 3 face-down + 1 face-up.

### Edge Cases
- If a player can't complete a war phase → they lose.
- If neither player has enough cards → the one who runs out first loses.
- If both run out simultaneously → draw.

---

## Project Requirements

- 2D scene
- **Fake Multiplayer** game: player vs computer (see below)
- Top-down GUI: player's card at the bottom, opponent's card at the top
- Support multiple screen sizes and aspect ratios
- User interaction: single tap to throw the next card
- Animate card movements

---

## Implementation Notes

- Card graphics can be downloaded from the internet
- Write clean code, maintain good design
- For animations: use a tweening engine or animation controller
- Unity version: **2022.3.21**
- Deployment to mobile not required — runs in Unity editor only

---

## Using Generative AI

Using generative AI is allowed. If a class was almost fully AI-generated, add a comment stating so.

---

## Fake Multiplayer

Implement a `FakeServer` class that:
- Emulates a server: client sends a request → server processes game logic **asynchronously** → returns result to client
- Handles all game state on the server side; client updates via API calls
- Bonus: emulate server edge-cases (network errors, timeouts)

### Focus
The focus is on **how the client handles server communication**. The server itself can be simple:

```csharp
public class FakeWarServer
{
    public async UniTask<Response> DrawCard()
    {
        // Draw with some delay
    }
}
```

### Client/Server Separation
- Separate client and server code into distinct classes/folders
- Share only minimal code between them (e.g., a few shared Enums)

---

## Delivery

- Upload code to a **public Git repository** and share the link
