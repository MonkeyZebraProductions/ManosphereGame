using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class SelectRandomString : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LocalisableText;
    //[SerializeField] LocalizedStringTable[] StringTables;
    LocalizedStringTable LocalizedStringTable;
    string LocalisedString, lastString;
    int tableIndex;
    public CircleTypes circleType;

    private void Awake()
    {
        //circleType = GetComponentInParent<CircleTypes>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if(LocalisedString!=lastString)
        {
            LocalisableText.text = LocalisedString;
        }
    }

    void OnEnable()
    {
         //circleType = GetComponentInParent<CircleTypes>();
         LocalizedStringTable = circleType.ChosenStuct.LocalizedStringTable;
         tableIndex = Random.Range(0, LocalizedStringTable.GetTable().Count);
         LocalizedStringTable.TableChanged += LoadStrings;  
    }

    void OnDisable()
    {
        LocalizedStringTable.TableChanged -= LoadStrings;
    }

    void LoadStrings(StringTable stringTable)
    {
        LocalisedString = GetLocalizedString(stringTable, tableIndex.ToString());
        Debug.Log(LocalisedString);
    }

    static string GetLocalizedString(StringTable table, string entryName)
    {
        // Get the table entry. The entry contains the localized string and Metadata
        var entry = table.GetEntry(entryName);
        return entry.GetLocalizedString(); // We can pass in optional arguments for Smart Format or String.Format here.
    }
}
