# Card War Game Animations

This document outlines all animations that need to be implemented for the Card War game based on the game rules in `CardWarGame.cs`.

## Core Gameplay Animations

### 1. Card Draw Animation
- **Player Card Draw**: Card sliding from player's deck pile to center play area
- **Opponent Card Draw**: Card sliding from opponent's deck pile to center play area
- **Card Flip**: Card flip animation transitioning from back to face
- **Timing**: Sequential draws (player first, then opponent) or simultaneous

### 2. Normal Round Win Animation
- **Comparison Visual**: Highlight or emphasize the winning card
- **Pot Award**: Cards moving from pot to winner's side pile
- **Winner Celebration**: Optional particles, glow, or visual feedback on winner's area
- **Duration**: ~1-2 seconds total

### 3. War Declaration Animation
- **"WAR!" Banner**: Display dramatic text/banner when cards are equal
- **Emphasis Effect**: Screen shake, flash, or zoom effect
- **Audio Cue Sync**: Dramatic sound effect timing
- **Duration**: ~0.5-1 second

### 4. War Cards Animation
- **Face-Down Cards**: 3 cards dealt from each player, sliding to pot area
- **Card Stacking**: Cards overlapping/stacking in the pot
- **Face-Up Card**: 1 final card dealt on top with flip animation
- **Sequence**: Show the dealing process (not instant)
- **Duration**: ~2-3 seconds for full war sequence

### 5. Recursive War Animation
- **Multiple Wars**: Support for consecutive war scenarios (war after war)
- **Pot Growth**: Visual indication of growing pot size
- **Tension Building**: Increasing intensity with each war
- **Chaining**: Smooth transition between multiple wars

## Deck Management Animations

### 6. Deck Refill Animation
- **Shuffle Visual**: Side pile cards mixing/shuffling effect
- **Card Transfer**: Cards moving from side pile back to deck pile
- **Deck Rebuild**: Deck pile growing/rebuilding
- **Trigger**: When deck is empty and side pile has cards
- **Duration**: ~1-2 seconds

### 7. Card Count Updates
- **Deck Pile Height**: Visual thickness/height changing based on card count
- **Side Pile Visualization**: Side pile updates when cards are added
- **Pot Indicator**: Pot count display updating
- **Smooth Transitions**: Gradual changes, not instant jumps

## Game State Animations

### 8. Game End Animations

#### Player Won
- Victory celebration
- Confetti or particle effects
- Winning banner/text display
- All cards moving to player's side

#### Opponent Won
- Defeat animation
- Dimming or desaturation of player area
- Loss indicator

#### Draw
- Stalemate animation
- Neutral visual feedback
- "Draw" text display

### 9. UI Feedback Animations
- **Card Count Displays**: Smooth number transitions
- **Pot Count Indicator**: Real-time pot size updates
- **Game State Transitions**: Fade or slide transitions between states
- **Button States**: Hover, press, and disabled states

## Optional Polish Animations

### 10. Card Hover/Selection Effects
- Card lift/elevation on hover
- Glow or outline effect
- Smooth transitions

### 11. Environmental Effects
- Screen shake on war declaration
- Background pulse on major events
- Dynamic lighting changes

### 12. Sound Effect Sync Points
- Card flip sound timing
- Card placement sound
- Win/lose sound effects
- War declaration sound
- Shuffle sound

### 13. Transition Delays
- Pause between card draws for readability
- Delay before resolving card comparison
- Wait time before clearing pot
- Pacing to ensure player can follow the action

## Animation Priority Levels

### High Priority (Core Gameplay)
1. Card Draw Animation
2. Card Flip Animation
3. Normal Round Win Animation
4. War Cards Animation
5. Pot Award Animation

### Medium Priority (Game Feel)
6. War Declaration Banner
7. Deck Refill Animation
8. Card Count Updates
9. Game End Animations

### Low Priority (Polish)
10. Environmental Effects
11. Card Hover Effects
12. Advanced Particle Systems
13. Screen Shake

## Technical Considerations

### Animation Sequences
- Use coroutines or async/await for sequencing
- Ensure animations can be sped up or skipped if desired
- Support for animation speed settings
- Queue system for multiple animations

### Performance
- Object pooling for card instances
- Efficient particle system usage
- Optimize for mobile if applicable

### Accessibility
- Option to reduce motion
- Option to skip or speed up animations
- Clear visual indicators regardless of animation state
