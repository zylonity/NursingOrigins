namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;


    public class IfBlinked : MonoBehaviour
    {

        bool sceneReady = false;

        public AudioSource audioSource;
        public PostProcessVolume camVol;

        Vignette vig;
        ChromaticAberration cA;
        ColorGrading cG;

        [Header(" --- Stage 1")]
        public GameObject Stage1;
        public AudioClip audOne;
        public float Stage1Time;
        public float maxVigAndCaIncS1;
        public GameObject blinkCanvasS11;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS1;
        eyeIcon blinkIconS11;

        [Header(" --- Stage 2")]
        public GameObject Stage2;
        public AudioClip audTwo;
        public float Stage2Time;
        public float maxVigAndCaIncS2;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS2;
        public CannonBall ballS21, ballS22, ballS23;

        [Header(" --- Stage 3")]
        public GameObject Stage3;
        public AudioClip audThree;
        public float Stage3Time;
        public float maxVigAndCaIncS3;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS3;

        [Header(" --- Stage 4")]
        public GameObject Stage4;
        public AudioClip audFour;
        public float Stage4Time;
        public float maxVigAndCaIncS4;
        [ColorUsageAttribute(false, true)]
        public Color colorShiftS4;


        int stage = 1;
        float counter = 0;
        float tCounter = 1;
        float maxSlow = 0.2f;

        float ogInt;
        Color ogCol;

        void OnEnable()
        {
            BlinkDect.onBlink += bBlinked;

            

            blinkIconS11 = blinkCanvasS11.GetComponent<eyeIcon>();
            camVol.profile.TryGetSettings(out vig);
            camVol.profile.TryGetSettings(out cA);
            camVol.profile.TryGetSettings(out cG);
            ogInt = cA.intensity.value;
            ogCol = cG.colorFilter.value;
        }

        private void Update()
        {

            if (stage == 1)
            {
                //print(counter + " " + tempC + " " + tCounter);
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                if (counter < Stage1Time)
                {

                    if (tCounter > maxSlow)
                        tCounter -= (counter / Stage1Time) / 100f;
                    else
                        tCounter = maxSlow;


                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS1, counter / Stage1Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS1, counter / Stage1Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS1, counter / Stage1Time);


                }
                else
                {
                    tCounter = maxSlow;
                    blinkCanvasS11.SetActive(true);
                    sceneReady = true;
                }
            }
            else if(stage == 2)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;

                if (counter < Stage2Time)
                {

                    if (tCounter > maxSlow)
                        tCounter -= (counter / Stage1Time) / 100f;
                    else
                        tCounter = maxSlow;


                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS2, counter / Stage2Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS2, counter / Stage2Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS2, counter / Stage2Time);

                    if (counter > Stage2Time / 2)
                    {
                        ballS21.Fire();
                        ballS22.Fire();
                        ballS23.Fire();
                    }



                }
                else
                {
                    tCounter = maxSlow;
                    blinkCanvasS11.SetActive(true);
                    sceneReady = true;
                }
            }
            else if (stage == 3)
            {
                counter += Time.unscaledDeltaTime;
                Time.timeScale = tCounter;
                if (counter < Stage2Time)
                {

                    if (tCounter > maxSlow)
                        tCounter -= (counter / Stage1Time) / 100f;
                    else
                        tCounter = maxSlow;


                    vig.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS2, counter / Stage2Time);
                    cA.intensity.Interp(ogInt, ogInt + maxVigAndCaIncS2, counter / Stage2Time);
                    cG.colorFilter.Interp(ogCol, colorShiftS2, counter / Stage2Time);


                }
                else
                {
                    tCounter = maxSlow;
                    blinkCanvasS11.SetActive(true);
                    sceneReady = true;
                }
            }
        }


        void OnDisable()
        {
            BlinkDect.onBlink -= bBlinked;
        }

        void bBlinked()
        {

            if (sceneReady) //&& blinkIconS11.onObj)
            {
                if (stage == 1)
                    Stage1End();
                else if (stage == 2)
                    Stage2End();
                else if (stage == 3)
                    Stage3End();
            }



        }

        void Stage1End()
        {
            Stage1.SetActive(false);
            Stage2.SetActive(true);
            audioSource.clip = audTwo;
            audioSource.Play();
            sceneReady = false;
            stage = 2;
            counter = 0;
            tCounter = 0.9f;
            maxSlow = 0.2f;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }

        void Stage2End()
        {
            Stage2.SetActive(false);
            Stage3.SetActive(true);
            audioSource.clip = audThree;
            audioSource.Play();
            sceneReady = false;
            stage = 3;
            counter = 0;
            tCounter = 0.8f;
            maxSlow = 0.2f;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }

        void Stage3End()
        {
            Stage3.SetActive(false);
            //Stage4.SetActive(true);
            audioSource.clip = audFour;
            audioSource.Play();
            sceneReady = false;
            stage = 4;
            counter = 0;
            tCounter = 1;
            maxSlow = 0.2f;
            ogInt = cA.intensity;
            ogCol = cG.colorFilter;
        }


    }
}


