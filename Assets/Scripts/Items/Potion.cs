namespace Items
{
    public class Potion
    {
        public PotionType type { get; }
        
        public float duration { get; }
        
        public Potion(PotionType type)
        {
            this.type = type;

            duration = type switch
            {
                PotionType.Health => 20f,
                PotionType.Speed => 50f,
                PotionType.Damage => 15f,
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
