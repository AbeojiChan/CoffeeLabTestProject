using UnityEngine;
using UnityEditor;

namespace KJD.Editor.BuildResonance
{
    [InitializeOnLoad]
    public static class BuildResonanceEngine
    {
        private const string COMPILING_FLAG = "KJD_BuildResonance_IsCompiling";

        // Les conventions de nommage absolues. 
        // L'extension (.wav, .mp3) n'a pas d'importance pour le scanner.
        private const string SUCCESS_FILE_NAME = "KJD_Success";
        private const string ERROR_FILE_NAME = "KJD_Error";

        static BuildResonanceEngine()
        {
            EditorApplication.update += MonitorCompilation;

            if (SessionState.GetBool(COMPILING_FLAG, false))
            {
                SessionState.SetBool(COMPILING_FLAG, false);
                if (!EditorUtility.scriptCompilationFailed) PlayFeedback(true);
            }
        }

        private static void MonitorCompilation()
        {
            bool isCompiling = EditorApplication.isCompiling;
            bool wasCompiling = SessionState.GetBool(COMPILING_FLAG, false);

            if (isCompiling && !wasCompiling)
            {
                SessionState.SetBool(COMPILING_FLAG, true);
            }
            else if (!isCompiling && wasCompiling)
            {
                SessionState.SetBool(COMPILING_FLAG, false);
                if (EditorUtility.scriptCompilationFailed) PlayFeedback(false);
            }
        }

        private static void PlayFeedback(bool isSuccess)
        {
            string targetName = isSuccess ? SUCCESS_FILE_NAME : ERROR_FILE_NAME;

            // Le traceur GPS : on cherche le nom strict, et on exige que ce soit un AudioClip
            string[] guids = AssetDatabase.FindAssets($"{targetName} t:AudioClip");

            // Si le fichier n'existe pas, on abandonne silencieusement. Zéro erreur console.
            if (guids.Length == 0) return;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            AudioClip clipToPlay = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

            if (clipToPlay == null) return;

            // Création du haut-parleur fantôme
            GameObject audioSpeaker = new GameObject("KJD_ResonanceSpeaker");
            audioSpeaker.hideFlags = HideFlags.HideAndDontSave;

            AudioSource source = audioSpeaker.AddComponent<AudioSource>();
            source.clip = clipToPlay;
            source.volume = 1f;
            source.Play();

            // Autodestruction asynchrone
            double startTime = EditorApplication.timeSinceStartup;
            float clipLength = clipToPlay.length;

            EditorApplication.CallbackFunction monitorPlayback = null;
            monitorPlayback = () =>
            {
                if (EditorApplication.timeSinceStartup - startTime >= clipLength)
                {
                    EditorApplication.update -= monitorPlayback;
                    if (audioSpeaker != null) GameObject.DestroyImmediate(audioSpeaker);
                }
            };

            EditorApplication.update += monitorPlayback;
        }
    }
}