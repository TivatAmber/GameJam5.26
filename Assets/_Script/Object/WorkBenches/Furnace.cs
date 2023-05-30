using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : BaseObject, IReceive, IGive
{
    List<GameObject> inMaterials = new();
    List<GameObject> inFuels = new();
    [SerializeField] List<GameObject> outputs = new();
    [SerializeField] List<Channel> channels = new();
    Channel outChannel;
    [SerializeField] CircleTimer timer;
    [SerializeField] int maxTime;
    [SerializeField] float maxInfluence;
    [SerializeField] Animator animator;
    #region IReceive
    public bool ReceiveObject(GameObject rec)
    {
        BaseObject baseObject = rec.GetComponent<BaseObject>();

        if (baseObject.HasTag(Feature.IsFuel)) inFuels.Add(rec);
        else if (baseObject.HasTag(Feature.IsMaterial)) inMaterials.Add(rec);
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
    public bool GiveObject(Kind which = 0)
    {
        if (which == 0)
        {
            if (inMaterials.Count > 0)
            {
                GameManager.Instance.nowGameObject = inMaterials[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inMaterials.RemoveAt(0);
                return true;
            }
            if (inFuels.Count > 0)
            {
                GameManager.Instance.nowGameObject = inFuels[0];
                GameManager.Instance.nowGameObject.SetActive(true);
                inFuels.RemoveAt(0);
                return true;
            }
        }
        else
        {
            Kind tmp;
            GameObject target = null;
            for (int i = 0; i < inMaterials.Count; i++)
            {
                tmp = BaseOpe.GetKind(inMaterials[i]);
                if (tmp == which && target == null)
                {
                    target = inMaterials[i];
                    inMaterials.RemoveAt(i);
                }
            }
            for (int i = 0; i < inFuels.Count; i++)
            {
                tmp = BaseOpe.GetKind(inFuels[i]);
                if (tmp == which && target == null)
                {
                    target = inFuels[i];
                    inFuels.RemoveAt(i);
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
        if (outputs.Count == 0)
        {
            timer.InterruptTimer();
        }
        else if (outputs.Count > 0)
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
        if (timer.Working)
        {
            foreach (GameObject obj in outputs)
            {
                BaseObject tmp = obj.GetComponent<BaseObject>();
                if (tmp != null) tmp.hardness += maxInfluence / maxTime * Time.deltaTime;
            }
        }
        animator.SetBool("isWorking", timer.Working);
    }

    private bool CheckSynthessisTable(out List<GameObject> ret)
    {
        ret = new();
        string excelPath = Path.Combine(Application.streamingAssetsPath, "SynthessisTable.xlsx");
        string excelSheetName = "Furnace";

        DataTable excelTableData = ReadExcel(excelPath, excelSheetName);

        int rows = excelTableData.Rows.Count;
        int columns = excelTableData.Columns.Count;

        var excelRowData = excelTableData.Rows;

        List<Kind> materials = new();
        foreach (GameObject obj in inMaterials)
        {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                materials.Add(tmp.Kind);
            }
        }
        materials.Sort();

        List<Kind> fuels = new();
        foreach (GameObject obj in inFuels) {
            BaseObject tmp;
            if ((tmp = obj.GetComponent<BaseObject>()) != null)
            {
                fuels.Add(tmp.Kind);
            }
        }
        fuels.Sort();

        List<Kind> tmpret;
        for (int i = 1; i < rows; i++)
        {
            List<Kind> targetMaterials = GetValues(excelRowData[i][0].ToString());
            List<Kind> targetFuels = GetValues(excelRowData[i][1].ToString());

            if (materials.SequenceEqual(targetMaterials) && fuels.SequenceEqual(targetFuels))
            {
                foreach (GameObject obj in inMaterials) Destroy(obj);
                foreach (GameObject obj in inFuels) Destroy(obj);
                ClearChannel();
                materials.Clear();
                fuels.Clear();
                inMaterials.Clear();
                inFuels.Clear();
                tmpret = GetValues(excelRowData[i][2].ToString());

                timer.StartTimer(maxTime);
                foreach (Kind x in tmpret) ret.Add(PrefabManager.Instance.GetObject(x));
                foreach (GameObject obj in ret) obj.SetActive(false);
                return true;
            }
        }
        return false;
    }
    private bool ClearChannel()
    {
        foreach(Channel cha in channels)
        {
            if (cha is InChannel)
            {
                var now = cha as InChannel;
                now.Clear();
            }
        }
        return true;
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
        foreach (string s in now)
        {
            if (s != "") ret.Add((Kind)Convert.ToInt32(s.Trim()));
        }
        ret.Sort();
        return ret;
    }
}
