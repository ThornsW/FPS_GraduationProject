using System;
using System.Collections;
using Assets.ProgressBars.Scripts;
using UnityEngine;

namespace Assets.ProgressBars.Demo.Scripts
{
    public class DemoSlider : MonoBehaviour {
        private const int MinPage = 1;
        private const int MaxPage = 17;

        public Camera Camera;
        private float _currentPosition;
        private float _currentPage;
        private GuiProgressBar[] _progressBars;
        private bool _isValueChanging;

        void Start () {
            _isValueChanging = true;
            _currentPage = 1;
            FindProgressBars();
        }

        void FindProgressBars ()
        {
            _progressBars = GameObject.Find (string.Format ("Page{0}", _currentPage)).GetComponentsInChildren<GuiProgressBar> ();
        }

        void Update () {
            if (Input.GetKeyDown("right")) {
                NextPage ();
            }

            if (Input.GetKeyDown ("left")) {
                PrevPage();
            }

            if (Input.GetKeyDown ("space")) {
                _isValueChanging = !_isValueChanging;
            }

            if (_isValueChanging) {
                foreach(var progressBar in _progressBars)
                {
                    progressBar.Value += 0.005f;
                    if (progressBar.Value > 1) progressBar.Value = 0;
                }
            }
        }

        public void NextPage()
        {
            if(_currentPage < MaxPage)
            {
                _currentPage += 1;
                _currentPosition += 8;
                FindProgressBars();
                StartCoroutine(MoveToPosition(new Vector3(_currentPosition, 0, -5), 0.1f, FinalizePos));
            }
        }

        public void PrevPage()
        {
            if(_currentPage > MinPage)
            {
                _currentPage -= 1;
                _currentPosition -= 8;
                FindProgressBars();
                StartCoroutine(MoveToPosition(new Vector3(_currentPosition, 0, -5), 0.1f, FinalizePos));
            }
        }

        private IEnumerator MoveToPosition(Vector3 newPosition, float time, Action doLast) {
            float elapsedTime = 0;
            Vector3 startingPos = Camera.transform.position;
            while (elapsedTime < time)
            {
                Camera.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
                doLast();
            }
        }

        private void FinalizePos ()
        {
            Camera.transform.position = new Vector3(_currentPosition, 0, -5);
        }
    }
}
