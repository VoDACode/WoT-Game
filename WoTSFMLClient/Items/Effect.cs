using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public enum CounterType { Numbers, Alphabet }
    public class Effect : BaseItem
    {
        private static readonly string _rootPath = @"Sources\PNG\Effects\";
        private DateTime lastAnimationTime = DateTime.MinValue;
        private string counter(int i) => _counterType == CounterType.Alphabet ? ((char)(65 + i)).ToString() : "0" + (i + 1).ToString();
        private CounterType _counterType;
        private bool isEnd = false;
        private int pointer = 0;
        private List<Sprite> effects;

        public event Action OnEndAnimation;

        public bool Looped { get; set; } = false;

        public Effect(Window window, string effectName, CounterType counterType = CounterType.Alphabet) : base(window)
        {
            _counterType = counterType;
            effects = new List<Sprite>();
            for (int i = 0; i < 24; i++)
            {
                if (File.Exists(Path.Join(_rootPath, $"{effectName}_{counter(i)}.png")))
                    effects.Add(new Sprite(new Texture(Path.Join(_rootPath, $"{effectName}_{counter(i)}.png"))));
                else
                    break;
            }
        }

        public override Vector2f Size
        {
            get => new Vector2f(effects.First().Texture.Size.X, effects.First().Texture.Size.Y);
            set
            {
                throw new NotImplementedException();
            }
        }
        public override PositionData Position
        {
            get => _position;
            set
            {
                _position = value;
                foreach (var effect in effects)
                    effect.Position = _positionInPixel;
            }
        }
        public Vector2f Origin
        {
            get => effects.First().Origin;
            set
            {
                foreach (var item in effects)
                    item.Origin = value;
            }
        }
        public float Rotation
        {
            get => effects.First().Rotation;
            set
            {
                foreach (var item in effects)
                    item.Rotation = value;
            }
        }

        public override void Loaded()
        {
            alignCalculate();
            base.Loaded();
        }

        public void ReloadEffect()
        {
            pointer = 0;
            isEnd = false;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (pointer >= effects.Count && !Looped)
            {
                if (!isEnd)
                {
                    OnEndAnimation?.Invoke();
                    isEnd = true;
                }
                return;
            }
            else if (pointer >= effects.Count)
                pointer = 0;
            if (lastAnimationTime == DateTime.MinValue)
                lastAnimationTime = DateTime.Now;
            else if (lastAnimationTime + TimeSpan.FromMilliseconds(85) > DateTime.Now)
            {
                target.Draw(effects[pointer]);
                return;
            }
            target.Draw(effects[pointer]);
            pointer++;
            lastAnimationTime = DateTime.Now;
        }
    }
}
