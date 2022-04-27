namespace Items
{
    public class Potion
    {
        public readonly PotionType type;
        public float duration;

        public Potion(PotionType type)
        {
            this.type = type;

            duration = type switch
            {
                PotionType.Health => 0.5f,
                PotionType.Speed => 5f,
                PotionType.Damage => 10f,
                _ => duration
            };
        }
    }
    
    public enum PotionType
    {
        None,
        Health,
        Speed,
        Damage
    }
}
