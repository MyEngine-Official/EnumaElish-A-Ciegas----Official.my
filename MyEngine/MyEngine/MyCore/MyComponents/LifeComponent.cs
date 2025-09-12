using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.MyCore.MyComponents
{
    public class LifeComponent
    {
        public int MaxLifePoints { get; set; } = 200;
        public int LifePoints { get; set; } = 200;
        public event Action<bool> OnLifePointsZero;
        public LifeComponent(int lifePoints) 
        {
            MaxLifePoints = lifePoints;
            LifePoints = lifePoints;
        }

        public void ClearLifePoints()
        {
            MaxLifePoints = 0;
            if (LifePoints <= 0)
                OnLifePointsZero.Invoke(true);
        }

        public void ResetLife()
        {
            LifePoints = MaxLifePoints;
        }

        public int GetLifePercentage()
        {
            return (int)((LifePoints / MaxLifePoints) * 100);
        }
        public void AddLifePoints(int extraPoints)
        {
            LifePoints += extraPoints;
            if (LifePoints <= 0)
                OnLifePointsZero.Invoke(true);
        }

        public void ReduceLifePoints(int points)
        {
            LifePoints -= points;
            if (LifePoints <= 0)
                OnLifePointsZero.Invoke(true);

        }
    }
}
