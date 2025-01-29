using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace SuperTiled2Unity.Editor
{
    public partial class TmxAssetImporter
    {
        private struct CurrentTileCustomProperties
        {
            public bool DisableCollider;
        }
        
        private HashSet<string> tilesToRemoveCollision;

        public void ResetCustomLayerData()
        {
            tilesToRemoveCollision = new HashSet<string>();
        }

        public IEnumerable<XElement> GetElementsInCustomOrder(XElement xMap)
        {
            var elements = xMap.Elements();
            Debug.Log(elements);
            return elements.OrderBy(x => string.IsNullOrEmpty(x.Attribute("class")?.Value));
        }

        public void ProcessCustomTileLayer(XElement xLayer)
        {
            var propertiesXElement = xLayer.Element("properties");
            if (propertiesXElement == null)
            {
                return;
            }
            
            var properties = CustomPropertyLoader.LoadCustomPropertyList(propertiesXElement);
            string target = properties.Find(x => x.m_Name == CustomStringConstants.Custom_Target)?.m_Value;
            string type = properties.Find(x => x.m_Name == CustomStringConstants.Custom_Type)?.m_Value;

            if (string.IsNullOrEmpty(target))
            {
                return;
            }
            if (string.IsNullOrEmpty(type))
            {
                return;
            }
            
            var xData = xLayer.Element("data");
            if (xData == null)
            {
                return;
            }

            Chunk chunk = new Chunk
            {
                Encoding = xData.GetAttributeAs<DataEncoding>("encoding"),
                XmlChunk = xData,
                X = 0,
                Y = 0,
                Width = m_MapComponent.m_Width,
                Height = m_MapComponent.m_Height
            };
            
            var tileIds = ReadTileIdsFromChunk(chunk);

            for (int i = 0; i < tileIds.Count; i++)
            {
                uint utId = tileIds[i];
                if (utId != 0)
                {
                    var cx = i % chunk.Width;
                    var cy = i / chunk.Width;

                    cx += chunk.X;
                    cy += chunk.Y;

                    string tileKey = GetTileKey(target, cx, cy);
                    
                    switch(type)
                    {
                        case CustomStringConstants.Custom_Type_RemoveColliders:
                            tilesToRemoveCollision.Add(tileKey);
                            break;
                    }
                }
            }
        }

        public bool IsCustomPropertyLayer(XElement xLayer)
        {
            var classStr = xLayer.Attribute("class");
            if (classStr == null)
            {
                return false;
            }

            return classStr.Value == "Custom";
        }

        private void PreProcessTileWithCustomLayersData(string layerName, int x, int y, 
            out CurrentTileCustomProperties customProperties)
        {
            customProperties = new CurrentTileCustomProperties();
            
            string tileKey = GetTileKey(layerName, x, y);
            
            customProperties.DisableCollider = tilesToRemoveCollision.Contains(tileKey);
        }

        private string GetTileKey(string layerName, int x, int y)
        {
            return $"{layerName}_({x},{y})";
        }
    }
}