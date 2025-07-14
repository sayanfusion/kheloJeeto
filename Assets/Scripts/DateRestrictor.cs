using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Dates;


/// <summary>
/// Sets the current date in a datepicker and restricts date selection for user from a given date to the current date.
/// </summary>
public class DateRestrictor : MonoBehaviour {
    [SerializeField, Range(0, 31)]
    private int _dateRange = 7;
    public DatePicker _datePickerFrom;
    public DatePicker _datePickerTo;

    void Start() {

        // Get the current date
        System.DateTime currentDate = System.DateTime.Now;
        SetDate(currentDate);

        _datePickerFrom.Config.DateRange.RestrictFromDate = true;
        _datePickerFrom.Config.DateRange.RestrictToDate = true;
        _datePickerTo.Config.DateRange.RestrictFromDate = true;
        _datePickerTo.Config.DateRange.RestrictToDate = true;
        _datePickerFrom.Config.DateRange.FromDate = currentDate.AddDays(-_dateRange);
        _datePickerFrom.Config.DateRange.ToDate = currentDate;
        _datePickerTo.Config.DateRange.FromDate = currentDate.AddDays(-_dateRange);
        _datePickerTo.Config.DateRange.ToDate = currentDate;
    }

    public void SetDate(System.DateTime date) {
        if (_datePickerFrom != null) {
            _datePickerFrom.SelectedDate = date; // Set the date in "From" DatePicker
        } else {
            Debug.LogError("DatePicker 'From' reference is missing!");
        }

        if (_datePickerTo != null) {
            _datePickerTo.SelectedDate = date; // Set the date in "To" DatePicker
        } else {
            Debug.LogError("DatePicker 'To' reference is missing!");
        }
    }
}
