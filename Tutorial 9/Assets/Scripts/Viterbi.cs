using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Viterbi
{
    public static List<char> GetPath(List<char> observedStates)
    {
        // Transition probabilities
        float p_ss = 0.8f;
        float p_sr = 0.2f;
        float p_rs = 0.4f;
        float p_rr = 0.6f;

        // Emission probabilities
        float p_sh = 0.8f;
        float p_sg = 0.2f;
        float p_rh = 0.4f;
        float p_rg = 0.6f;

        // Initial probabilities
        float p_s = 2 / 3f;
        float p_r = 1 / 3f;

        List<Vector2> probabilities = new List<Vector2>();
        var resultingStates = new List<char>();

        // Set first node probabilities because the first node is using initial probability
        // All other nodes in the chain are based only on the previous state
        if(observedStates[0] == 'H')
        {
            var firstNodeHappyState = new Vector2(p_s * p_sh, p_r * p_rh);
            probabilities.Add(firstNodeHappyState);
        }
        else
        {
            var firstNodeGrumpyState = new Vector2(p_s * p_sg, p_r * p_rg);
            probabilities.Add(firstNodeGrumpyState);
        }

        // Loop through all observed states and compute the state probabilities
        for (int i = 1; i < observedStates.Count; i++)
        {
            float yesterday_sunny = probabilities[i-1].x;
            float yesterday_rainy = probabilities[i-1].y;

            // Take current state and store max values
            if (observedStates[i] == 'H')
            {
                float today_sunny = Mathf.Max(yesterday_sunny * p_ss * p_sh, yesterday_rainy * p_rs * p_sh);
                float today_rainy = Mathf.Max(yesterday_sunny * p_sr * p_rh, yesterday_rainy * p_rr * p_rh);
                var currentStateValues = new Vector2(today_sunny, today_rainy);
                probabilities.Add(currentStateValues);
            }
            else
            {
                float today_sunny = Mathf.Max(yesterday_sunny * p_ss * p_sg, yesterday_rainy * p_rs * p_sg);
                float today_rainy = Mathf.Max(yesterday_sunny * p_sr * p_rg, yesterday_rainy * p_rr * p_rg);
                var currentStateValues = new Vector2(today_sunny, today_rainy);
                probabilities.Add(currentStateValues);
            }
        }

        // Find the optimal path (with max probabilities)
        foreach (var item in probabilities)
        {
            if(item.x > item.y)
            {
                resultingStates.Add('S');
            }
            else
            {
                resultingStates.Add('R');
            }
        }

        return resultingStates;
    }

}
