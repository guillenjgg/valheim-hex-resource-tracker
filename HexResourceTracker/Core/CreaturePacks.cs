using System;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    // Groups creatures of one kind that stand close together, so a pack or herd shows as a single pin.
    internal static class CreaturePacks
    {
        // Each creature joins the first pack whose first member is within the radius (on the map plane), else starts a new pack.
        internal static List<List<T>> Group<T>(IEnumerable<T> creatures, Func<T, float> x, Func<T, float> z, float radius)
        {
            var packs = new List<List<T>>();
            float radiusSquared = radius * radius;

            foreach (T creature in creatures)
            {
                List<T> pack = null;

                if (radius > 0f)
                {
                    foreach (List<T> candidate in packs)
                    {
                        float dx = x(candidate[0]) - x(creature);
                        float dz = z(candidate[0]) - z(creature);

                        if (dx * dx + dz * dz <= radiusSquared)
                        {
                            pack = candidate;
                            break;
                        }
                    }
                }

                if (pack == null)
                {
                    pack = new List<T>();
                    packs.Add(pack);
                }

                pack.Add(creature);
            }

            return packs;
        }
    }
}
