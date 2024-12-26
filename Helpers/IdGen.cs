using IdGen;
using System;

namespace DragonAPI.Helpers
{
    public static class IdGen
    {
        public static long GenId()
        {
            // Let's say we take August 1st 2021 as our epoch
            var epoch = new DateTime(2021, 8, 1, 0, 0, 0, DateTimeKind.Utc);
            // Create an ID with 41 bits for timestamp, 6 for generator-id and 16 for sequence
            var structure = new IdStructure(41, 6, 16);
            // Prepare options
            var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));
            var generator = new IdGenerator(0, options);
            return generator.CreateId();
        }
    }
}
