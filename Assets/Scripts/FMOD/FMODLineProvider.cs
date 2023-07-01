using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class FMODLineProvider : LineProviderBehaviour
{
    [SerializeField] private FMODDialogue fMODDialogue;

    [Language]
    public string textLanguageCode = System.Globalization.CultureInfo.CurrentCulture.Name;

    private void Start()
    {
        if (fMODDialogue == null) { fMODDialogue = GetComponent<FMODDialogue>(); }
    }

    public override LocalizedLine GetLocalizedLine(Yarn.Line line)
    {
        var text = YarnProject.GetLocalization(textLanguageCode).GetLocalizedString(line.ID);
        fMODDialogue.PlayDialogue(line.ID);

        return new LocalizedLine()
        {
            TextID = line.ID,
            RawText = text,
            Substitutions = line.Substitutions,
            Metadata = YarnProject.lineMetadata.GetMetadata(line.ID),
        };
    }

    public override void PrepareForLines(IEnumerable<string> lineIDs)
    {
        // No-op; text lines are always available
    }

    public override bool LinesAvailable => true;

    public override string LocaleCode => textLanguageCode;
}
