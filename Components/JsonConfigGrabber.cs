using JLL.Components.Filters;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static JLL.Components.JModConfigGrabber;

namespace JLL.Components
{
    public class JsonConfigGrabber : MonoBehaviour
    {
        [Header("-=-DEPRICATED-=-")]
        public string modAuthor = "";
        public string modName = "";

        [FormerlySerializedAs("checkAllOnAwake")]
        public bool checkAllOnEnable = true;

        [Header("Mod Properties")]
        public PropertyCheck<bool, CheckFilter>[] BoolChecks = new PropertyCheck<bool, CheckFilter>[0];
        public PropertyCheck<int, IntFilter>[] IntChecks = new PropertyCheck<int, IntFilter>[0];
        public PropertyCheck<float, NumericFilter>[] FloatChecks = new PropertyCheck<float, NumericFilter>[0];
        public PropertyCheck<string, NameFilter>[] StringChecks = new PropertyCheck<string, NameFilter>[0];

        [Header("Check All Properties Result")]
        public UnityEvent CheckSuccess = new UnityEvent();
        public UnityEvent CheckFailure = new UnityEvent();

        public void Start()
        {
            JModConfigGrabber grabber = gameObject.AddComponent<JModConfigGrabber>();

            grabber.modAuthor = modAuthor;
            grabber.modName = modName;
            grabber.checkAllOnEnable = checkAllOnEnable;
            grabber.BoolChecks = BoolChecks;
            grabber.IntChecks = IntChecks;
            grabber.FloatChecks = FloatChecks;
            grabber.CheckSuccess = CheckSuccess;
            grabber.CheckFailure = CheckFailure;

            Destroy(this);
        }
    }
}
