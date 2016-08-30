using Orleans.Consensus.Contract.Log;
using System.IO;
using Wire;

namespace Orleans.Consensus.Log
{
    public class WireSerializer<T> : ISerializer<T> where T : new()
    {
        Serializer serializer;

        public WireSerializer()
        {

            var logEntrySurrogate = Surrogate.Create<LogEntry<T>, MutableLogEntry<T>>(
                original => new MutableLogEntry<T>() {Operation = original.Operation, Id = new MutableLogEntryId { Index = original.Id.Index, Term = original.Id.Term } }, 
                surrogate => new LogEntry<T>(new LogEntryId(surrogate.Id.Index, surrogate.Id.Term), surrogate.Operation));
            
            var options = new SerializerOptions(surrogates: new[] { logEntrySurrogate });

            serializer = new Serializer(options);
        }

        public T Deserialize(Stream stream)
        {
            return serializer.Deserialize<T>(stream);
        }

        public void Serialize(T value, Stream stream)
        {
            serializer.Serialize(value, stream);
        }
    }
}
