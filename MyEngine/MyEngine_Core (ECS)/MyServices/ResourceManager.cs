using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyEngine_Core.MyServices
{
    /// <summary>
    /// Manages and caches game resources
    /// </summary>
    public class ResourceManager : IDisposable
    {
        private ContentManager _content;
        private readonly Dictionary<string, Texture2D> _textures;
        private readonly Dictionary<string, SoundEffect> _soundEffects;
        private readonly Dictionary<string, Song> _songs;
        private readonly Dictionary<string, SpriteFont> _fonts;
        private readonly Dictionary<string, Effect> _effects;
        private bool _isDisposed;

        /// <summary>
        /// Creates a new resource manager
        /// </summary>
        public ResourceManager(ContentManager content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _textures = new Dictionary<string, Texture2D>();
            _soundEffects = new Dictionary<string, SoundEffect>();
            _songs = new Dictionary<string, Song>();
            _fonts = new Dictionary<string, SpriteFont>();
            _effects = new Dictionary<string, Effect>();
        }

        /// <summary>
        /// Default constructor for compatibility
        /// </summary>
        public ResourceManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            _soundEffects = new Dictionary<string, SoundEffect>();
            _songs = new Dictionary<string, Song>();
            _fonts = new Dictionary<string, SpriteFont>();
            _effects = new Dictionary<string, Effect>();
        }

        /// <summary>
        /// Sets the content manager (for late initialization)
        /// </summary>
        public void SetContentManager(ContentManager content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// Loads a texture, using cache if already loaded
        /// </summary>
        public Texture2D LoadTexture(string path)
        {
            if (_textures.ContainsKey(path))
                return _textures[path];

            if (_content == null)
                throw new InvalidOperationException("ContentManager not set. Call SetContentManager first.");

            try
            {
                var texture = _content.Load<Texture2D>(path);
                _textures[path] = texture;
                return texture;
            }
            catch (ContentLoadException ex)
            {
                throw new ContentLoadException($"Failed to load texture '{path}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a sound effect, using cache if already loaded
        /// </summary>
        public SoundEffect LoadSoundEffect(string path)
        {
            if (_soundEffects.ContainsKey(path))
                return _soundEffects[path];

            if (_content == null)
                throw new InvalidOperationException("ContentManager not set. Call SetContentManager first.");

            try
            {
                var sound = _content.Load<SoundEffect>(path);
                _soundEffects[path] = sound;
                return sound;
            }
            catch (ContentLoadException ex)
            {
                throw new ContentLoadException($"Failed to load sound effect '{path}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a song, using cache if already loaded
        /// </summary>
        public Song LoadSong(string path)
        {
            if (_songs.ContainsKey(path))
                return _songs[path];

            if (_content == null)
                throw new InvalidOperationException("ContentManager not set. Call SetContentManager first.");

            try
            {
                var song = _content.Load<Song>(path);
                _songs[path] = song;
                return song;
            }
            catch (ContentLoadException ex)
            {
                throw new ContentLoadException($"Failed to load song '{path}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a sprite font, using cache if already loaded
        /// </summary>
        public SpriteFont LoadFont(string path)
        {
            if (_fonts.ContainsKey(path))
                return _fonts[path];

            if (_content == null)
                throw new InvalidOperationException("ContentManager not set. Call SetContentManager first.");

            try
            {
                var font = _content.Load<SpriteFont>(path);
                _fonts[path] = font;
                return font;
            }
            catch (ContentLoadException ex)
            {
                throw new ContentLoadException($"Failed to load font '{path}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads an effect/shader, using cache if already loaded
        /// </summary>
        public Effect LoadEffect(string path)
        {
            if (_effects.ContainsKey(path))
                return _effects[path];

            if (_content == null)
                throw new InvalidOperationException("ContentManager not set. Call SetContentManager first.");

            try
            {
                var effect = _content.Load<Effect>(path);
                _effects[path] = effect;
                return effect;
            }
            catch (ContentLoadException ex)
            {
                throw new ContentLoadException($"Failed to load effect '{path}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads a generic content type
        /// </summary>
        public T Load<T>(string path)
        {
            if (_content == null)
                throw new InvalidOperationException("ContentManager not set. Call SetContentManager first.");

            return _content.Load<T>(path);
        }

        /// <summary>
        /// Unloads a specific texture from cache
        /// </summary>
        public void UnloadTexture(string path)
        {
            if (_textures.ContainsKey(path))
            {
                _textures[path]?.Dispose();
                _textures.Remove(path);
            }
        }

        /// <summary>
        /// Unloads all cached resources
        /// </summary>
        public void UnloadAll()
        {
            foreach (var texture in _textures.Values)
                texture?.Dispose();
            _textures.Clear();

            foreach (var sound in _soundEffects.Values)
                sound?.Dispose();
            _soundEffects.Clear();

            foreach (var song in _songs.Values)
                song?.Dispose();
            _songs.Clear();

            foreach (var effect in _effects.Values)
                effect?.Dispose();
            _effects.Clear();

            _fonts.Clear();
        }

        /// <summary>
        /// Gets statistics about loaded resources
        /// </summary>
        public ResourceStatistics GetStatistics()
        {
            return new ResourceStatistics
            {
                TextureCount = _textures.Count,
                SoundEffectCount = _soundEffects.Count,
                SongCount = _songs.Count,
                FontCount = _fonts.Count,
                EffectCount = _effects.Count,
                TotalResourceCount = _textures.Count + _soundEffects.Count + 
                                    _songs.Count + _fonts.Count + _effects.Count
            };
        }

        /// <summary>
        /// Checks if a resource is already loaded
        /// </summary>
        public bool IsLoaded(string path, ResourceType type)
        {
            return type switch
            {
                ResourceType.Texture => _textures.ContainsKey(path),
                ResourceType.SoundEffect => _soundEffects.ContainsKey(path),
                ResourceType.Song => _songs.ContainsKey(path),
                ResourceType.Font => _fonts.ContainsKey(path),
                ResourceType.Effect => _effects.ContainsKey(path),
                _ => false
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    UnloadAll();
                    _content?.Unload();
                }
                _isDisposed = true;
            }
        }
    }

    /// <summary>
    /// Resource type enumeration
    /// </summary>
    public enum ResourceType
    {
        Texture,
        SoundEffect,
        Song,
        Font,
        Effect
    }

    /// <summary>
    /// Resource statistics
    /// </summary>
    public class ResourceStatistics
    {
        public int TextureCount { get; set; }
        public int SoundEffectCount { get; set; }
        public int SongCount { get; set; }
        public int FontCount { get; set; }
        public int EffectCount { get; set; }
        public int TotalResourceCount { get; set; }

        public override string ToString()
        {
            return $"Resources - Textures: {TextureCount}, Sounds: {SoundEffectCount}, " +
                   $"Songs: {SongCount}, Fonts: {FontCount}, Effects: {EffectCount}, Total: {TotalResourceCount}";
        }
    }
}
