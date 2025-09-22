using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyEngine.MyCore.MyComponents;

namespace MyEngine.MyCore.Events
{
    // Eventos Base
    public abstract class GameEvent
    {
        public DateTime Timestamp { get; } = DateTime.Now;
        public bool IsHandled { get; set; } = false;
    }

    // ================================
    // EVENTOS DE INPUT
    // ================================
    public class KeyPressedEvent : GameEvent
    {
        public Keys Key { get; set; }
        public int EntityId { get; set; }
    }
    public class KeyReleasedEvent : GameEvent
    {
        public Keys Key { get; set; }
        public int EntityId { get; set; }
    }

    public class GamePadButtonEvent : GameEvent
    {
        public Buttons Button { get; set; }
        public int PlayerId { get; set; }
        public int EntityId { get; set; }
        public bool IsPressed { get; set; }
    }

    public class TouchEvent : GameEvent
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public int EntityId { get; set; }
        public bool IsPressed { get; set; }
    }

    // ================================
    // EVENTOS DE FÍSICA
    // ================================
    public class CollisionEnterEvent : GameEvent
    {
        public int Entity1Id { get; set; }
        public int Entity2Id { get; set; }
        public Vector2 CollisionPoint { get; set; }
        public string Entity1Tag { get; set; }
        public string Entity2Tag { get; set; }
    }
    public class CollisionExitEvent : GameEvent
    {
        public int Entity1Id { get; set; }
        public int Entity2Id { get; set; }
    }
    public class TriggerEnterEvent : GameEvent
    {
        public int TriggerId { get; set; }
        public int EntityId { get; set; }
        public string TriggerTag { get; set; }
        public string EntityTag { get; set; }
    }

    // ================================
    // EVENTOS DE ENTIDADES
    // ================================
    public class EntitySpawnedEvent : GameEvent
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public Vector2 Position { get; set; }
    }

    public class EntityDestroyedEvent : GameEvent
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public Vector2 LastPosition { get; set; }
    }

    public class EntityMovedEvent : GameEvent
    {
        public int EntityId { get; set; }
        public Vector2 OldPosition { get; set; }
        public Vector2 NewPosition { get; set; }
    }

    public class  EntityDirectionEvent: GameEvent 
    {
        public int EntityId { get; set; }
        public Vector2 Direction { get; set; }
    }

    // ================================
    // EVENTOS DE GAMEPLAY
    // ================================
    public class PlayerHealthChangedEvent : GameEvent
    {
        public int PlayerId { get; set; }
        public int OldHealth { get; set; }
        public int NewHealth { get; set; }
        public int MaxHealth { get; set; }
        public int Damage { get; set; }
        public string DamageSource { get; set; }
    }

    public class PlayerDiedEvent : GameEvent
    {
        public int PlayerId { get; set; }
        public Vector2 DeathPosition { get; set; }
        public string CauseOfDeath { get; set; }
    }

    public class ItemCollectedEvent : GameEvent
    {
        public int PlayerId { get; set; }
        public int ItemEntityId { get; set; }
        public string ItemType { get; set; }
        public int Value { get; set; }
        public Vector2 Position { get; set; }
    }

    public class ScoreChangedEvent : GameEvent
    {
        public int PlayerId { get; set; }
        public int OldScore { get; set; }
        public int NewScore { get; set; }
        public int PointsAdded { get; set; }
        public string Reason { get; set; }
    }

    // ================================
    // EVENTOS DE ANIMACIÓN
    // ================================
    public class AnimationStartedEvent : GameEvent
    {
        public int EntityId { get; set; }
        public AnimationAction AnimationName { get; set; }
        public bool IsLooping { get; set; }
    }

    public class AnimationCompletedEvent : GameEvent
    {
        public int EntityId { get; set; }
        public string AnimationName { get; set; }
    }

    public class AnimationLoopedEvent : GameEvent
    {
        public int EntityId { get; set; }
        public string AnimationName { get; set; }
        public int LoopCount { get; set; }
    }

    // ================================
    // EVENTOS DE AUDIO
    // ================================
    public class PlaySoundEvent : GameEvent
    {
        public string SoundName { get; set; }
        public float Volume { get; set; } = 1.0f;
        public float Pitch { get; set; } = 0.0f;
        public float Pan { get; set; } = 0.0f;
        public bool Loop { get; set; } = false;
        public Vector2? Position { get; set; } // Para audio posicional
    }

    public class PlayMusicEvent : GameEvent
    {
        public string MusicName { get; set; }
        public bool Loop { get; set; } = true;
        public float FadeInTime { get; set; } = 0f;
    }

    public class StopMusicEvent : GameEvent
    {
        public float FadeOutTime { get; set; } = 0f;
    }

    // ================================
    // EVENTOS DE UI
    // ================================
    public class UIButtonClickedEvent : GameEvent
    {
        public string ButtonName { get; set; }
        public int ButtonId { get; set; }
        public Vector2 ClickPosition { get; set; }
    }

    public class UIWindowOpenedEvent : GameEvent
    {
        public string WindowName { get; set; }
    }

    public class UIWindowClosedEvent : GameEvent
    {
        public string WindowName { get; set; }
    }

    // ================================
    // EVENTOS DE ESCENA
    // ================================
    public class SceneChangeRequestedEvent : GameEvent
    {
        public string NewSceneName { get; set; }
        public string TransitionType { get; set; }
        public Dictionary<string, object> SceneData { get; set; }
    }

    public class SceneChangedEvent : GameEvent
    {
        public string OldSceneName { get; set; }
        public string NewSceneName { get; set; }
    }

    public class GamePausedEvent : GameEvent
    {
        public string PauseReason { get; set; }
    }

    public class GameResumedEvent : GameEvent { }

    // ================================
    // EVENTOS DE SISTEMA
    // ================================
    public class SaveGameEvent : GameEvent
    {
        public string SaveSlot { get; set; }
        public Dictionary<string, object> GameData { get; set; }
    }

    public class LoadGameEvent : GameEvent
    {
        public string SaveSlot { get; set; }
    }

    public class GameOverEvent : GameEvent
    {
        public int FinalScore { get; set; }
        public TimeSpan PlayTime { get; set; }
        public string Reason { get; set; }
    }
}
