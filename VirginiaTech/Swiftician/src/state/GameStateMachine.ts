/**
 * AI-GENERATED FILE — Claude (Anthropic), PM4 milestone
 * Prompt: "Implement the State behavioral pattern from PM3 design for game session phases"
 *
 * GameStateMachine — implements the State behavioral design pattern
 * described in PM3.
 *
 * The game session transitions through three phases:
 *   LOBBY  → ACTIVE → ENDED
 *
 * Behaviour is phase-dependent:
 *   - LOBBY:  players can join; answers are ignored
 *   - ACTIVE: answers are processed; new joins are rejected
 *   - ENDED:  everything is read-only; results are displayed
 */

import { eventBus, GameEvents } from '../events/eventBus'

export type GamePhase = 'LOBBY' | 'ACTIVE' | 'ENDED'

interface GameState {
  phase: GamePhase
  canJoin(): boolean
  canSubmitAnswer(): boolean
  canAdvanceQuestion(): boolean
}

class LobbyState implements GameState {
  phase: GamePhase = 'LOBBY'
  canJoin() {
    return true
  }
  canSubmitAnswer() {
    return false
  }
  canAdvanceQuestion() {
    return false
  }
}

class ActiveState implements GameState {
  phase: GamePhase = 'ACTIVE'
  canJoin() {
    return false
  }
  canSubmitAnswer() {
    return true
  }
  canAdvanceQuestion() {
    return true
  }
}

class EndedState implements GameState {
  phase: GamePhase = 'ENDED'
  canJoin() {
    return false
  }
  canSubmitAnswer() {
    return false
  }
  canAdvanceQuestion() {
    return false
  }
}

const stateInstances: Record<GamePhase, GameState> = {
  LOBBY: new LobbyState(),
  ACTIVE: new ActiveState(),
  ENDED: new EndedState(),
}

export class GameStateMachine {
  private currentState: GameState
  private lobbyId: number

  constructor(lobbyId: number, initialPhase: GamePhase = 'LOBBY') {
    this.lobbyId = lobbyId
    this.currentState = stateInstances[initialPhase]
  }

  get phase(): GamePhase {
    return this.currentState.phase
  }

  canJoin(): boolean {
    return this.currentState.canJoin()
  }

  canSubmitAnswer(): boolean {
    return this.currentState.canSubmitAnswer()
  }

  canAdvanceQuestion(): boolean {
    return this.currentState.canAdvanceQuestion()
  }

  transitionTo(phase: GamePhase): void {
    const previous = this.currentState.phase
    if (previous === phase) return

    this.currentState = stateInstances[phase]

    if (phase === 'ACTIVE') {
      eventBus.emit(GameEvents.GAME_STARTED, { lobbyId: this.lobbyId })
    } else if (phase === 'ENDED') {
      eventBus.emit(GameEvents.GAME_ENDED, { lobbyId: this.lobbyId })
    }
  }
}
