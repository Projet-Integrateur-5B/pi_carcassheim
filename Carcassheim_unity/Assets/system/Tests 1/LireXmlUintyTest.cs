using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Linq;
using System.IO;



namespace Tests
{
    public class LireXmlUintyTest
    {
        
        // A Test behaves as an ordinary method
        [Test]
        public void TuilesReadedEqualGeneratedTuiles()
        {
            String file = "Assets/system/infos.xml";
            LireXml xmlObj;
            xmlObj = new LireXml(file);
            LireXml.CreateXMLFile();
            LireXml.ReadXml();
            int expectedKeyDicoTuile = 2;
            int actualKeyDicoTuile =Tuile.DicoTuiles.ElementAt(0).Key;
            Debug.Log(actualKeyDicoTuile);
            Assert.AreEqual(expectedKeyDicoTuile, actualKeyDicoTuile);

            File.Delete(file);

        }

    }
}