using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Test
{

    class BinSearchTreeSerialization<Dict>
    {

        private Stream stream;

        public BinSearchTreeSerialization(Stream stream)
        {
            this.stream = stream;
        }

        public void Serialize(Dict dictionary)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Position = 0;
            formatter.Serialize(stream, dictionary);
        }

        public Dict Deserialize()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Position = 0;
            Dict tree = (Dict)formatter.Deserialize(stream);
            return tree;
        }

    }

}
