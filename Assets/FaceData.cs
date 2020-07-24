using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace Azw.Iyi
{

    [RequireComponent(typeof(ARFace))]
    public class FaceData : MonoBehaviour
    {
        ARFace face;
        Text text;

        ARKitFaceSubsystem m_ARKitFaceSubsystem;

        void Awake()
        {
            face = GetComponent<ARFace>();
        }
        void Start() {
            text = GameObject.Find("Text").GetComponent<Text>();
        }

        void OnEnable()
        {
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null)
            {
                m_ARKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
            }
            face.updated += OnUpdated;
        }

        void OnDisable()
        {
            face.updated -= OnUpdated;
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            using (var blendShapes = m_ARKitFaceSubsystem.GetBlendShapeCoefficients(face.trackableId, Allocator.Temp))
            {
                var t = "";
                foreach (var featureCoefficient in blendShapes)
                {
                    t += (featureCoefficient.blendShapeLocation.ToString() + featureCoefficient.coefficient.ToString()) + "\n";
                }
                text.text = t;
            }
        }
    }
}