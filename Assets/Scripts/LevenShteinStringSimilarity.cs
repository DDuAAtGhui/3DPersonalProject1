using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevenShteinStringSimilarity : MonoBehaviour
{
    #region Leven Shtein 문자열 유사성 비교 알고리즘

    /// <summary>
    /// 가장 이름이 비슷한 상태 반환
    /// </summary>
    /// <param name="targetStateName"></param>
    /// <param name="playerStates"></param>
    /// <param name="Percentage"></param>
    /// <returns></returns>
    public static PlayerStates FindMostSimilarState(string targetStateName, PlayerStates[] playerStates, out float Percentage)
    {
        if (string.IsNullOrEmpty(targetStateName) || playerStates == null || playerStates.Length == 0)
        {
            Percentage = 0;
            return null;
        }

        PlayerStates bestMatchState = null;
        float matchingPercentage = 0.00f;

        foreach (PlayerStates state in playerStates)
        {
            string stateName = state.ToString();
            float similarity = CalculateSimilarity_string(targetStateName, stateName);

            if (similarity > matchingPercentage)
            {
                matchingPercentage = similarity;
                bestMatchState = state;
            }
        }

        Debug.Log("bestMatchState : " + bestMatchState + "    " + matchingPercentage + "%");
        Percentage = matchingPercentage;
        return bestMatchState;
    }

    public static string FindMostSimilarString(string targetString, string[] stringArray)
    {
        if (string.IsNullOrEmpty(targetString) || stringArray == null || stringArray.Length == 0)
        {
            return string.Empty;
        }

        string bestMatchString = string.Empty;
        float matchingPercentage = 0.00f;

        foreach (string str in stringArray)
        {
            float similarity = CalculateSimilarity_string(targetString, str);

            if (similarity > matchingPercentage)
            {
                matchingPercentage = similarity;
                bestMatchString = str;
            }
        }

        Debug.Log("bestMatchString : " + bestMatchString + "    " + matchingPercentage + "%");
        return bestMatchString;
    }
    public static float CalculateSimilarity_string(string A, string B)
    {
        //지정된 두 숫자중 더 큰 숫자 반환
        int maxLength = Math.Max(A.Length, B.Length);
        int editDistance = ComputeEditDistance(A, B);

        float similarityPercentage = 100.00f * (1.0f - (float)editDistance / maxLength);
        return similarityPercentage;
    }

    //Leven Shtein 문자열 유사성 비교 알고리즘
    public static int ComputeEditDistance(string A, string B)
    {
        int[,] distance = new int[A.Length + 1, B.Length + 1];

        for (int i = 0; i <= A.Length; i++)
        {
            for (int j = 0; j <= B.Length; j++)
            {
                if (i == 0)
                    distance[i, j] = j;

                else if (j == 0)
                    distance[i, j] = i;

                else
                {
                    int substitutionCost = (A[i - 1] == B[j - 1]) ? 0 : 1;

                    distance[i, j] = Math.Min
                        (distance[i - 1, j] + 1,
                          Math.Min
                          (
                            distance[i, j - 1] + 1,
                            distance[i - 1, j - 1] + substitutionCost
                           )
                        );
                }
            }
        }

        return distance[A.Length, B.Length];
    }
    #endregion

}
