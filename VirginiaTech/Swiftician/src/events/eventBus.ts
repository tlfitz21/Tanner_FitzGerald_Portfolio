/**
 * AI-GENERATED FILE — Claude (Anthropic), PM4 milestone
 * Prompt: "Implement the event-based architecture from PM3 design document"
 *
 * EventBus — lightweight pub/sub system that drives Swiftician's
 * event-based architecture.
 *
 * Events emitted by the game:
 *   GameCreated   — teacher creates a new game room
 *   PlayerJoined  — a student joins a lobby
 *   GameStarted   — teacher starts the game session
 *   QuestionBroadcast — a new question is pushed to all players
 *   AnswerSubmitted   — a student submits an answer
 *   ScoreCalculated   — scoring module finishes grading an answer
 *   RoundComplete     — all answers received or timer expired
 *   GameEnded         — game session is over
 */

type EventCallback = (...args: unknown[]) => void

class EventBus {
  private listeners: Record<string, EventCallback[]> = {}

  on(event: string, callback: EventCallback): void {
    if (!this.listeners[event]) {
      this.listeners[event] = []
    }
    this.listeners[event].push(callback)
  }

  off(event: string, callback: EventCallback): void {
    const callbacks = this.listeners[event]
    if (!callbacks) return
    this.listeners[event] = callbacks.filter((cb) => cb !== callback)
  }

  emit(event: string, ...args: unknown[]): void {
    const callbacks = this.listeners[event]
    if (!callbacks) return
    for (const cb of callbacks) {
      try {
        cb(...args)
      } catch (err) {
        console.error(`[EventBus] Error in handler for "${event}":`, err)
      }
    }
  }

  /** Remove all listeners for a specific event, or all events if no name given. */
  clear(event?: string): void {
    if (event) {
      delete this.listeners[event]
    } else {
      this.listeners = {}
    }
  }
}

export const eventBus = new EventBus()

// Event name constants
export const GameEvents = {
  GAME_CREATED: 'GameCreated',
  PLAYER_JOINED: 'PlayerJoined',
  GAME_STARTED: 'GameStarted',
  QUESTION_BROADCAST: 'QuestionBroadcast',
  ANSWER_SUBMITTED: 'AnswerSubmitted',
  SCORE_CALCULATED: 'ScoreCalculated',
  ROUND_COMPLETE: 'RoundComplete',
  GAME_ENDED: 'GameEnded',
} as const
