using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RandomSortTuilesTests
    {
        [Test]
        public void Tuile_a_tirerTest()
        {
            int id = 0, x = 8, actualResult = 0;
            Dictionary<int, int> map = new Dictionary<int, int>()
        {
            { 0, 2},
            { 1, 1},
            { 2, 3},
            { 3, 1},
            { 4, 9},
            { 5, 10},
            { 6, 11},
            { 7, 13},
            { 8, 14}
        };
            actualResult = Thread_serveur_jeu.tuile_a_tirer(id, x, map);
            int expectedResult = 4;
            Debug.Log(actualResult);
            Debug.Log(expectedResult);
            Assert.AreEqual(actualResult, expectedResult);

        }


    }
}