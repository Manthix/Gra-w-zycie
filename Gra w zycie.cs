using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Gra
{

    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("============ START\n");
            
            List<string> input = new List<string>()
            {
                ".........",
                ".........",
                ".........",
                ".........",
                ".........",

            };
            
            try
            {
                IReader reader = new Reader();
                IGrid grid = reader.Read(input);
                Console.WriteLine(grid.Display());

                ITurn turn = new Turn();
                for(int i=0; i<10; i++){
                    Console.WriteLine($"============ TURN {i+1}\n");
                    turn.DoTurn(grid);
                }
            }
            catch(NotImplementedException nie)
            {
                Console.WriteLine("Something is not ready: " + nie.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("============ END");
        }
    }
    
    //=========INTERFACES
    
    public interface IDisplayable
    {
        string Display();
    }
    public interface IField : IDisplayable
    {
        bool Lion{get;set;}
        bool Crocodile{get;set;}
        bool Elephant{get;set;}
        bool Antelope{get;set;}
        bool Sex{get;set;}
        bool Moves{get;  set;}
        ICords Cords{get;}
    }
    
    public interface ICords
    {
        int X{get;set;}
        int Y{get;set;}
    }
    
    public interface IGrid : IDisplayable
    {

        void AddField(IField field);
        void Move();
        void Eat();
        void Copulate();

    }
    
    public interface IReader
    {
        IGrid Read(List<string> input);
    }
    
    public interface ITurn 
    {
        void DoTurn(IGrid grid);
    }
    
    //=========IMPLEMENTATIONS
    
    public class Cords : ICords
    {
        public int X{get;set;}
        public int Y{get;set;}
    }
    
    public class Turn : ITurn
    { 
        public void DoTurn(IGrid grid)
        {
            grid.Move();
            grid.Eat();
            grid.Copulate();
            Console.WriteLine(grid.Display());
        }
    }
    public class Reader : IReader
    {
        public IGrid Read(List<string> input)
        {
            int sizeX = input.Count;
            if(sizeX == 0)
            {
                throw new Exception("Empty input");
            }
            
            int sizeY = input[0].Length;
            
            IGrid grid = new Grid(sizeX, sizeY);
            int x = 0;
            foreach(string line in input)
            {
                int y = 0;
                foreach(char c in line)
                {
                    IField field = CreateField(c);
                    field.Cords.X = x;
                    field.Cords.Y = y;
                    
                    grid.AddField(field);
                    
                    y++;
                }
                x++;
            }
            return grid;
        }
        
        private IField CreateField(char c)
        {
            return new Field(c);
        }
    }
        
    public class Grid : IGrid 
    {
        private readonly List<List<IField>> _fields;
        private readonly int _sizeX;
        private readonly int _sizeY;
        
        public Grid(int sizeX, int sizeY)
        {
            _fields = new List<List<IField>>();
            _sizeX = sizeX;
            _sizeY = sizeY;
            SetSize(sizeX, sizeY);
        }
        public void AddField(IField field)
        {
            ICords cords = field.Cords;
            _fields[cords.X][cords.Y] = field;
        }
        public string Display()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach(List<IField> row in _fields)
            {
                foreach(IField f in row)
                {
                    sb.Append(f.Display());
                }
                sb.AppendLine();
            }
            
            return sb.ToString();
        }
        private void SetSize(int x, int y)
        {
            for(int i = 0; i < x; i++)
            {
                List<IField> row = new List<IField>();
                for(int j = 0; j < y; j++)
                {
                    row.Add(null);
                }
                _fields.Add(row);
            }
        }
        private List<IField> GetNeighboorFieldsForMoves(IField field)
        {
            List<IField> result = new List<IField>();
            //góra
            if( (field.Cords.X - 1 >= 0) && (field.Crocodile || field.Elephant)  )
            {
                IField fieldNext = _fields[field.Cords.X - 1][field.Cords.Y];
                result.Add(fieldNext);
            }
            //prawo
            if( (field.Cords.Y + 1 < _sizeY) && (field.Lion || field.Elephant || field.Antelope) )
            {
                IField fieldNext = _fields[field.Cords.X][field.Cords.Y + 1];
                result.Add(fieldNext);
            }
            //dół
            if( (field.Cords.X + 1 < _sizeX) && (field.Crocodile || field.Elephant)  )
            {
                IField fieldNext = _fields[field.Cords.X + 1][field.Cords.Y];
                result.Add(fieldNext);
            }
            //lewo
            if( (field.Cords.Y - 1 >= 0) && (field.Lion || field.Elephant)  )
            {
                IField fieldNext = _fields[field.Cords.X][field.Cords.Y - 1];
                result.Add(fieldNext);
            }
            
            return result;
        }
        private List<IField> GetNeighboorFields(IField field)
        {
            List<IField> result = new List<IField>();
            if( field.Cords.Y + 1 < _sizeY  )
            {
                IField fieldNext = _fields[field.Cords.X][field.Cords.Y + 1];
                result.Add(fieldNext);
            }
            if( field.Cords.Y - 1 >= 0  )
            {
                IField fieldNext = _fields[field.Cords.X][field.Cords.Y - 1];
                result.Add(fieldNext);
            }
            if( field.Cords.X + 1 < _sizeX  )
            {
                IField fieldNext = _fields[field.Cords.X + 1][field.Cords.Y];
                result.Add(fieldNext);
            }
            if( field.Cords.X - 1 >= 0  )
            {
                IField fieldNext = _fields[field.Cords.X - 1][field.Cords.Y];
                result.Add(fieldNext);
            }
            return result;
        }
        private bool IsWay(IField f)
        {
            if(!f.Lion && !f.Elephant && !f.Crocodile && !f.Antelope)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        private bool IsPartner(IField f,IField n)
        {
            if((f.Lion==n.Lion && f.Crocodile==n.Crocodile && f.Elephant==n.Elephant && f.Antelope==n.Antelope) && n.Sex==false)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        private void RestartMoves(IGrid grid)
        {
            foreach(List<IField> row in _fields)
            {
                foreach(IField f in row)
                {
                    if(f.Lion || f.Crocodile || f.Elephant || f.Antelope)
                    {
                        f.Moves=true;
                    }
                }
            }
        }
        private bool RandomBool()
        {
            var rand = new Random();
            if(rand.NextDouble() >= 0.5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Move()
        {
            RestartMoves(this);
            for(int i = 0; i<=_sizeY+_sizeX; i++){
            foreach(List<IField> row in _fields)
            {
                foreach(IField f in row)
                {
                    if(f.Moves){
                        List<IField> Neighboor = GetNeighboorFieldsForMoves(f);
                        foreach(IField n in Neighboor)
                        {
                            if(IsWay(n) && f.Moves)
                            {
                                n.Lion = f.Lion;
                                n.Crocodile = f.Crocodile;
                                n.Elephant = f.Elephant;
                                n.Antelope = f.Antelope;
                                n.Sex = f.Sex;
                                n.Moves = false;

                                f.Lion= false;
                                f.Crocodile = false;
                                f.Elephant = false;
                                f.Antelope = false;
                                f.Sex= false;
                            }
                        }

                        Neighboor = null;
                    }
                }
            }
            }
        }
        public void Eat()
        {
            RestartMoves(this);
            foreach(List<IField> row in _fields)
            {
                foreach(IField f in row)
                {
                    if(f.Lion || f.Crocodile)
                    {
                        List<IField> Neighboor = GetNeighboorFields(f);
                        foreach(IField n in Neighboor)
                        {
                           if(n.Antelope && f.Moves)
                           {
                               n.Antelope = false;
                               n.Sex = false;
                               f.Moves = false;
                           }
                        }
                    }
                }
            }
        }
        public void Copulate()
        {
            RestartMoves(this);
            foreach(List<IField> row in _fields)
            {
                foreach(IField f in row)
                {
                    if(f.Sex && f.Moves)
                    {
                        List<IField> Neighboor = GetNeighboorFields(f);
                        foreach(IField n in Neighboor)
                        {
                            if(IsPartner(f, n))
                            {
                                foreach(IField n2 in Neighboor)
                                {   
                                    if(IsWay(n2) && f.Moves)
                                    {
                                        n2.Lion = f.Lion;
                                        n2.Crocodile = f.Crocodile;
                                        n2.Elephant = f.Elephant;
                                        n2.Antelope = f.Antelope;
                                        n2.Sex = RandomBool();

                                        n2.Moves = false;
                                        f.Moves = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class Field : IField
    {
        public Field()
        {
            Lion = false;
            Crocodile = false;
            Elephant = false;
            Antelope = false;
            Sex = false;
            Moves = true;
            Cords = new Cords();
        }
        public bool Lion{get; set;}
        public bool Crocodile{get; set;}
        public bool Elephant{get; set;}
        public bool Antelope{get; set;}
        public bool Moves{get; set;}
        public bool Sex{get; set;}
        public ICords Cords{get;}
        
        public Field(char c)
        {
            Lion = false;
            Crocodile = false;
            Elephant = false;
            Antelope = false;
            Sex = false;
            Moves = false;
            Cords = new Cords();
            
            switch(c){
                case 'L': 
                
                    Lion=true;
                    Sex = false;
                break;
                case 'l':
                
                    Lion=true;
                    Sex = true;
                break;
                case 'K':
                
                    Crocodile = true;
                    Sex = false;
                break;
                case 'k':
                
                    Crocodile = true;
                    Sex = true;
                break;
                case 'S':
                
                    Elephant = true;
                    Sex = false;
                break;
                case 's':
                
                    Elephant = true;
                    Sex = true;
                break;
                case 'A':
                
                    Antelope = true;
                    Sex = false;
                break;
                case 'a':
                
                    Antelope = true;
                    Sex = true;
                break;
                case '.':
                    break;
                
                default:
                    throw new Exception($"Nierozpoznane pole : {c}.");
            }

        }
        public string Display()
        {
            if(Lion){
                if(Sex==false)
                {
                    return "L";
                }
                else
                {
                    return "l";
                }
            }
            else if(Crocodile){
                if(Sex==false)
                {
                    return "K";
                }
                else
                {
                    return "k";
                }
            }
            else if(Elephant){
                if(Sex==false)
                {
                    return "S";
                }
                else
                {
                    return "s";
                }
            }
            else if(Antelope){
                if(Sex==false)
                {
                    return "A";
                }
                else
                {
                    return "a";
                }
            }
            else
            {
                return ".";
            }
        }
    }
}