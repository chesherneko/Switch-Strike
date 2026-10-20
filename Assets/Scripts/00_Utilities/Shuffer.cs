using UnityEngine;

public static class Shuffer
{
    public static void Shuffle<T>(this T[] deck)
    {
        var maxCount = deck.Length - 1;

        for (int i = maxCount; i > 0; i--)
        {
            int randIdx = Random.Range(0, i + 1);

            T temp = deck[i];

            deck[i] = deck[randIdx];
            deck[randIdx] = temp;
        }
    }
}
