using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [SerializeField] Dictionary<int, GameObject> prefabTable = new();
    [SerializeField] List<GameObject> prefabList = new();
    int featureNumber = 12;

    string excelPath;
    string excelSheet;
    string prefabPath;
    DataTable dataTable;
    int rows, columns;
    DataRowCollection dataRows;
    public GameObject GetObject(int id)
    {
        GameObject newObject = null;

        for (int i = 1; i < rows; i++)
        {
            int nowId;
            if (!Int32.TryParse(dataRows[i][0].ToString(), out nowId)) continue;
            if (nowId == id)
            {
                // Prefab
                newObject = Instantiate(prefabTable[id]);
                if (newObject == null) continue;
                // Component
                BaseObject baseObject = newObject.GetComponent<BaseObject>();
                if (baseObject == null)
                {
                    Destroy(newObject);
                    continue;
                }
                // ID
                baseObject.ID = id;
                // Kind
                baseObject.Kind = (Kind)Enum.Parse(typeof(Kind), prefabTable[id].name);
                // Temperature
                ReadValue(dataRows[i][2].ToString(), ref baseObject.temperature);
                // Carbon
                ReadValue(dataRows[i][3].ToString(), ref baseObject.carbon);
                // Impurities
                ReadValue(dataRows[i][4].ToString(), ref baseObject.impurities);
                // Crispness
                ReadValue(dataRows[i][5].ToString(), ref baseObject.crispness);
                // Hardness
                ReadValue(dataRows[i][6].ToString(), ref baseObject.hardness);
            }
        }

        return newObject;
    }
    public GameObject GetObject(Kind kind)
    {
        return GetObject((int)kind);
    }
    private void Start()
    {
        prefabPath = "_Prefab";
        excelPath = Path.Combine(Application.streamingAssetsPath, "Cards.xlsx");
        excelSheet = "Cards";
        dataTable = ReadExcel(excelPath, excelSheet);
        rows = dataTable.Rows.Count;
        columns = dataTable.Columns.Count;
        dataRows = dataTable.Rows;
        for (int i = 1; i < rows; i++)
        {
            string Name = dataRows[i][1].ToString();
            string nowPath = prefabPath + "\\" + Name;
            //Debug.Log(nowPath);
            GameObject prefab = Resources.Load<GameObject>(nowPath);
            if (prefab == null) continue;
            // Component
            BaseObject baseObject = prefab.GetComponent<BaseObject>();
            if (baseObject == null)
            {
                Destroy(prefab);
                continue;
            }
            // ID
            baseObject.ID = Convert.ToInt32(dataRows[i][0].ToString());
            // Name
            prefab.name = Name;
            // Kind
            baseObject.Kind = (Kind)Enum.Parse(typeof(Kind), Name);
            // Features 
            for (int j = 0; j < featureNumber; j++)
            {
                Feature tag = (Feature)Enum.Parse(typeof(Feature), dataRows[0][7 + j].ToString());
                if (1 == Convert.ToInt32(dataRows[i][7 + j].ToString()))
                {
                    if (!baseObject.HasTag(tag))
                        baseObject.AddTag(tag);
                } 
                else
                {
                    baseObject.RemoveTag(tag);
                }
            }

            TMPro.TextMeshProUGUI text = baseObject.canvas.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            text.text = "      " + dataRows[i][7 + featureNumber].ToString();
            
            
            prefabList.Add(prefab);
            prefabTable.Add(baseObject.ID, prefab);
        }
    }
    DataTable ReadExcel(string excelPath, string excelSheet)
    {
        DataSet ret = null;
        using (FileStream fileStream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

            ret = excelReader.AsDataSet();
        }
        return ret.Tables[excelSheet];
    }
    private void ReadValue(string str, ref float val)
    {
        float value1 = 0.0f, value2 = 0.0f;
        if (IsNumberic(str, out value1))
        {
            val = value1;
        }
        else if (IsRange(str, out value1, out value2))
        {
            val = UnityEngine.Random.Range(value1, value2);
        }
        else
        {
            val = 0;
        }
    }
    private bool IsNumberic(string str, out float result)
    {
        bool isNum;
        isNum = float.TryParse(str, out result);
        return isNum;
    }
    private bool IsRange(string str, out float val1, out float val2)
    {
        bool isRange = false;
        string[] tmp = str.Split('~');
        if (tmp.Length == 2) isRange = float.TryParse(tmp[0], out val1) & float.TryParse(tmp[1], out val2);
        else val1 = val2 = 0;
        return isRange;
    }
}
