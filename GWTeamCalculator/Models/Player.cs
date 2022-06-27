namespace GWTeamCalculator
{
    public class Player
    {
        public Player(string name, float might)
        {
            Name = name;
            Might = might;
        }

        public string Name { get; }
        public float Might { get; }
     
        public override string ToString()
        {
            return $"Name: {Name} Might: {Might}";
        }
    }
}
