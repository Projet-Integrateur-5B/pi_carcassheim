using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Linq;


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
            Assert.AreEqual(actualResult, expectedResult);

        }
        [Test]
        public void Random_sort_tuilesTest()
        {
            int nbTuile = 60, minimumResult = 1;
            List<int> list = null;
            list = new List<int>();
            list =Thread_serveur_jeu.Random_sort_tuiles(nbTuile);
            int actualNbOfElements = list.Count;
            Assert.GreaterOrEqual(actualNbOfElements,minimumResult);
        }
        [Test]
        public void tirageTroisTuilesTest()
        {
            List<int> expected = null;
            expected = new List<int>();
            expected.Add(5);
            expected.Add(6);
            expected.Add(7);
            List<int> actual = null;
            actual = new List<int>();
            List<int> list = null;
            list=new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            list.Add(6);
            list.Add(7);
            actual=Thread_serveur_jeu.tirageTroisTuiles(list);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void suppTuileChoisieTest()
        {
            List<int> expected = null;
            expected = new List<int>();
            expected.Add(55);
            expected.Add(32);
            expected.Add(2);
            List<int> actual = null;
            actual = new List<int>();
            List<int> tuiles = null;
            tuiles = new List<int>();
            tuiles.Add(15);
            tuiles.Add(55);
            tuiles.Add(32);
            tuiles.Add(4);
            tuiles.Add(2);
            int idTuileToDelete = 4;//on veut supprimer le id 4
            actual = Thread_serveur_jeu.suppTuileChoisie(tuiles, idTuileToDelete);
            idTuileToDelete = 15;// aussi le 15
            actual = Thread_serveur_jeu.suppTuileChoisie(tuiles, idTuileToDelete);
            Assert.IsTrue(expected.SequenceEqual(actual));

        }
    }
}