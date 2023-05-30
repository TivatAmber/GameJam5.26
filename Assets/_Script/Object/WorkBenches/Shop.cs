using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Shop : BaseObject, IReceive, IGive
{
    List<GameObject> inPeople = new();
    List<GameObject> inBlades = new();
    List<GameObject> outputs = new();
    [SerializeField] List<Channel> channels = new();
    Channel outChannel;
    #region IReceive
    public bool ReceiveObject(GameObject rec)
    {
        BaseObject baseObject = rec.GetComponent<BaseObject>();

        if (baseObject.HasTag(Feature.IsPeople)) inPeople.Add(rec);
        else if (baseObject.HasTag(Feature.IsBlade) || baseObject.Kind == Kind.FloatCrystle) inBlades.Add(rec);
        else return false;
        rec.SetActive(false);


        if (outputs.Count == 0)
        {
            CheckSynthessisTable(out outputs);
        }
        return true;
    }
    #endregion
    #region IGive
    public bool GiveObject(Kind which)
    {
        if (which == 0)
        {
            if (inPeople.Count > 0)
            {
                GameManager.Instance.nowGameObject = inPeople[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inPeople.RemoveAt(0);
                return true;
            }
            if (inBlades.Count > 0)
            {
                GameManager.Instance.nowGameObject = inBlades[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inBlades.RemoveAt(0);
                return true;
            }
        }
        else
        {
            Kind tmp;
            GameObject target = null;
            for (int i = 0; i < inPeople.Count; i++)
            {
                tmp = BaseOpe.GetKind(inPeople[i]);
                if (tmp == which && target == null)
                {
                    target = inPeople[i];
                    inPeople.RemoveAt(i);
                }
            }
            for (int i = 0; i < inBlades.Count; i++)
            {
                tmp = BaseOpe.GetKind(inBlades[i]);
                if (tmp == which && target == null)
                {
                    target = inBlades[i];
                    inBlades.RemoveAt(i);
                }
            }
            if (target == null) return false;
            GameManager.Instance.nowGameObject = target;
            GameManager.Instance.nowGameObject.SetActive(true);
        }
        return true;
    }
    #endregion
    public bool OutObject()
    {
        if (outputs.Count == 0) return false;
        GameManager.Instance.nowGameObject = outputs[0];
        GameManager.Instance.nowGameObject.SetActive(true);
        outputs.RemoveAt(0);
        return true;
    }
    private bool ClearChannel()
    {
        foreach (Channel cha in channels)
        {
            if (cha is InChannel)
            {
                var now = cha as InChannel;
                now.Clear();
            }
        }
        return true;
    }
    new private void Start()
    {
        base.Start();
        foreach (Channel cha in channels)
        {
            if (cha is OutChannel)
            {
                outChannel = cha as OutChannel;
            }
        }
    }
    private void Update()
    {
        if (outputs.Count > 0)
        {
            GameObject obj = outputs[0];
            var a = obj.transform.Find("Icon");
            Texture tmp = null;
            if (a != null)
            {
                var b = a.Find("RawImage");
                if (b != null)
                {
                    var c = b.gameObject.GetComponent<RawImage>();
                    if (c != null)
                    {
                        tmp = c.texture;
                    }
                }
            }
            outChannel.ChangeImage(tmp);
        }
    }

    private bool CheckSynthessisTable(out List<GameObject> ret)
    {
        ret = new();
        string excelPath = Path.Combine(Application.streamingAssetsPath, "SynthessisTable.xlsx");
        string excelSheetName = "Shop";

        DataTable excelTableData = ReadExcel(excelPath, excelSheetName);

        int rows = excelTableData.Rows.Count;
        int columns = excelTableData.Columns.Count;

        var excelRowData = excelTableData.Rows;

        List<Kind> people = new();
        foreach (GameObject obj in inPeople)
        {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                people.Add(tmp.Kind);
            }
        }
        people.Sort();

        List<Kind> blades = new();
        foreach (GameObject obj in inBlades)
        {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                blades.Add(tmp.Kind);
            }
        }
        blades.Sort();

        List<Kind> tmpret;
        for (int i = 1; i < rows; i++)
        {
            List<Kind> targetPeople = GetValues(excelRowData[i][0].ToString());
            List<Kind> targetBlades = GetValues(excelRowData[i][1].ToString());

            if (people.SequenceEqual(targetPeople) && blades.SequenceEqual(targetBlades))
            {
                for (int j = 0; j < people.Count; j++)
                {
                    People tmp = inPeople[j].GetComponent<People>();
                    Blade tmp2 = inBlades[j].GetComponent<Blade>();
                    if (tmp != null && tmp2 != null)
                    {
                        tmp.GetBlade(tmp2);
                        break;
                    }
                    FloatCrystle tmp3 = inBlades[j].GetComponent<FloatCrystle>();
                    if (tmp != null && tmp3 != null)
                    {
                        tmp.GetFloatCrystle(tmp3);
                        break;
                    }
                }
                ClearChannel();
                blades.Clear();
                inBlades.Clear();
                tmpret = GetValues(excelRowData[i][2].ToString());
                foreach (Kind x in people) ret.Add(PrefabManager.Instance.GetObject(x));
                foreach (Kind x in tmpret) ret.Add(PrefabManager.Instance.GetObject(x));
                inPeople.Clear();
                people.Clear();
                foreach (GameObject obj in ret) obj.SetActive(false);
                return true;
            }
        }
        return false;
    }
    private T FindInterface<T>(GameObject gameObject)
        where T : class
    {
        MonoBehaviour[] tmp = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour obj in tmp)
        {
            if (obj is T)
            {
                return obj as T;
            }
        }
        return null;
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

    List<Kind> GetValues(string str)
    {
        List<Kind> ret = new();
        string[] now = str.Split(',');
        int res;
        foreach (string s in now)
        {
            if (int.TryParse(s, out res)) ret.Add((Kind)res);
        }
        ret.Sort();
        return ret;
    }
}
