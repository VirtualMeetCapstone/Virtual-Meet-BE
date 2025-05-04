using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Database.MongoDb_Entities
{
    public class WhiteBoardEntity 
    {
        public string RoomId { get; set; }
        public List<DrawingAction> DrawActions { get; set; } = new List<DrawingAction>();
    }

    public class DrawingAction
    {
        public string Type { get; set; } // "start", "draw", "clear"
        public double X { get; set; }
        public double Y { get; set; }
        public double ToX { get; set; }
        public double ToY { get; set; }
        public string Color { get; set; }
        public int lineWidth {  get; set; }
    }
}