using ReactiveUI;
using System;
using System.Data.Linq.Mapping;

namespace WiTrainingSuite.Model
{
    public partial class DocumentResult : ReactiveObject
    {
        private int? _DOC_ID;
        public int? DOC_ID
        {
            get { return _DOC_ID; }
            set { this.RaiseAndSetIfChanged(ref _DOC_ID, value); }
        }

        private string _DOC_CODE;
        public string DOC_CODE
        {
            get { return _DOC_CODE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_CODE, value); }
        }

        private string _DOC_TITLE;
        public string DOC_TITLE
        {
            get { return _DOC_TITLE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_TITLE, value); }
        }

        private int? _DOC_ISSUE;
        public int? DOC_ISSUE
        {
            get { return _DOC_ISSUE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_ISSUE, value); }
        }

        private DateTime? _DOC_ISSUEDATE;
        public DateTime? DOC_ISSUEDATE
        {
            get { return _DOC_ISSUEDATE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_ISSUEDATE, value); }
        }

        private string _DOC_RA;
        public string DOC_RA
        {
            get { return _DOC_RA; }
            set { this.RaiseAndSetIfChanged(ref _DOC_RA, value); }
        }

        private string _DOC_TASK_DESCRIPTION;
        public string DOC_TASK_DESCRIPTION
        {
            get { return _DOC_TASK_DESCRIPTION; }
            set { this.RaiseAndSetIfChanged(ref _DOC_TASK_DESCRIPTION, value); }
        }

        private string _DOC_RISK_DESCRIPTION;
        public string DOC_RISK_DESCRIPTION
        {
            get { return _DOC_RISK_DESCRIPTION; }
            set { this.RaiseAndSetIfChanged(ref _DOC_RISK_DESCRIPTION, value); }
        }

        private int? _DOCTAG_ID;
        public int? DOCTAG_ID
        {
            get { return _DOCTAG_ID; }
            set { this.RaiseAndSetIfChanged(ref _DOCTAG_ID, value); }
        }

        private string _DOCTAG_NAME;
        public string DOCTAG_NAME
        {
            get { return _DOCTAG_NAME; }
            set { this.RaiseAndSetIfChanged(ref _DOCTAG_NAME, value); }
        }
    }

    public partial class EmployeeResult : ReactiveObject
    {
        private int? _EMP_ID;
        public int? EMP_ID
        {
            get { return _EMP_ID; }
            set { this.RaiseAndSetIfChanged(ref _EMP_ID, value); }
        }

        private string _EMP_FNAME;
        public string EMP_FNAME
        {
            get { return _EMP_FNAME; }
            set { this.RaiseAndSetIfChanged(ref _EMP_FNAME, value); }
        }

        private string _EMP_LNAME;
        public string EMP_LNAME
        {
            get { return _EMP_LNAME; }
            set { this.RaiseAndSetIfChanged(ref _EMP_LNAME, value); }
        }

        private string _EMP_INITIAL;
        public string EMP_INITIAL
        {
            get { return _EMP_INITIAL; }
            set { this.RaiseAndSetIfChanged(ref _EMP_INITIAL, value); }
        }

        private string _EMP_EMAIL = string.Empty;
        public string EMP_EMAIL
        {
            get { return _EMP_EMAIL; }
            set { this.RaiseAndSetIfChanged(ref _EMP_EMAIL, value); }
        }

        private string _EMP_NOTES = string.Empty;
        public string EMP_NOTES
        {
            get { return _EMP_NOTES; }
            set { this.RaiseAndSetIfChanged(ref _EMP_NOTES, value); }
        }

        private bool _EMP_ON_TRAINING_PROGRAMME;
        public bool EMP_ON_TRAINING_PROGRAMME
        {
            get { return _EMP_ON_TRAINING_PROGRAMME; }
            set { this.RaiseAndSetIfChanged(ref _EMP_ON_TRAINING_PROGRAMME, value); }
        }

        private string _SHIFT_NAME = string.Empty;
        public string SHIFT_NAME
        {
            get { return _SHIFT_NAME; }
            set { this.RaiseAndSetIfChanged(ref _SHIFT_NAME, value); }
        }

        private string _DEPT_NAME = string.Empty;
        public string DEPT_NAME
        {
            get { return _DEPT_NAME; }
            set { this.RaiseAndSetIfChanged(ref _DEPT_NAME, value); }
        }

        private string _VAR_NAME = string.Empty;
        public string VAR_NAME
        {
            get { return _VAR_NAME; }
            set { this.RaiseAndSetIfChanged(ref _VAR_NAME, value); }
        }

        private string _VAR_NAME_TRAINING = string.Empty;
        public string VAR_NAME_TRAINING
        {
            get { return _VAR_NAME_TRAINING; }
            set { this.RaiseAndSetIfChanged(ref _VAR_NAME_TRAINING, value); }
        }

        private string _ROLE_NAME = string.Empty;
        public string ROLE_NAME
        {
            get { return _ROLE_NAME; }
            set { this.RaiseAndSetIfChanged(ref _ROLE_NAME, value); }
        }

        private int? _SHIFT_ID;
        public int? SHIFT_ID
        {
            get { return _SHIFT_ID; }
            set { this.RaiseAndSetIfChanged(ref _SHIFT_ID, value); }
        }

        private int? _DEPT_ID;
        public int? DEPT_ID
        {
            get { return _DEPT_ID; }
            set { this.RaiseAndSetIfChanged(ref _DEPT_ID, value); }
        }

        private int? _VAR_ID;
        public int? VAR_ID
        {
            get { return _VAR_ID; }
            set { this.RaiseAndSetIfChanged(ref _VAR_ID, value); }
        }

        private int? _VAR_ID_TRAINING;
        public int? VAR_ID_TRAINING
        {
            get { return _VAR_ID_TRAINING; }
            set { this.RaiseAndSetIfChanged(ref _VAR_ID_TRAINING, value); }
        }
    }

    public partial class SkillResult : ReactiveObject
    {
        private int? _SKILL_ID;
        public int? SKILL_ID
        {
            get { return _SKILL_ID; }
            set { this.RaiseAndSetIfChanged(ref _SKILL_ID, value); }
        }

        private string _SKILL_NAME;
        public string SKILL_NAME
        {
            get { return _SKILL_NAME; }
            set { this.RaiseAndSetIfChanged(ref _SKILL_NAME, value); }
        }
    }

    public partial class DeptResult : ReactiveObject
    {
        private int? _DEPT_ID;
        public int? DEPT_ID
        {
            get { return _DEPT_ID; }
            set { this.RaiseAndSetIfChanged(ref _DEPT_ID, value); }
        }

        private string _DEPT_NAME;
        public string DEPT_NAME
        {
            get { return _DEPT_NAME; }
            set { this.RaiseAndSetIfChanged(ref _DEPT_NAME, value); }
        }
    }

    public partial class ShiftResult : ReactiveObject
    {
        private int? _SHIFT_ID;
        public int? SHIFT_ID
        {
            get { return _SHIFT_ID; }
            set { this.RaiseAndSetIfChanged(ref _SHIFT_ID, value); }
        }

        private string _SHIFT_NAME;
        public string SHIFT_NAME
        {
            get { return _SHIFT_NAME; }
            set { this.RaiseAndSetIfChanged(ref _SHIFT_NAME, value); }
        }
    }

    public partial class DocTagResult : ReactiveObject
    {
        private int? _DOCTAG_ID;
        public int? DOCTAG_ID
        {
            get { return _DOCTAG_ID; }
            set { this.RaiseAndSetIfChanged(ref _DOCTAG_ID, value); }
        }

        private string _DOCTAG_NAME;
        public string DOCTAG_NAME
        {
            get { return _DOCTAG_NAME; }
            set { this.RaiseAndSetIfChanged(ref _DOCTAG_NAME, value); }
        }
    }

    public partial class RoleResult : ReactiveObject
    {
        private int? _ROLE_ID;
        public int? ROLE_ID
        {
            get { return _ROLE_ID; }
            set { this.RaiseAndSetIfChanged(ref _ROLE_ID, value); }
        }

        private string _ROLE_NAME;
        public string ROLE_NAME
        {
            get { return _ROLE_NAME; }
            set { this.RaiseAndSetIfChanged(ref _ROLE_NAME, value); }
        }

        private bool? _ROLE_HAS_EMAIL = false;
        public bool? ROLE_HAS_EMAIL
        {
            get { return _ROLE_HAS_EMAIL; }
            set { this.RaiseAndSetIfChanged(ref _ROLE_HAS_EMAIL, value); }
        }
    }

    public partial class VariantResult : ReactiveObject
    {
        private int? _VAR_ID;
        public int? VAR_ID
        {
            get { return _VAR_ID; }
            set { this.RaiseAndSetIfChanged(ref _VAR_ID, value); }
        }

        private string _VAR_NAME;
        public string VAR_NAME
        {
            get { return _VAR_NAME; }
            set { this.RaiseAndSetIfChanged(ref _VAR_NAME, value); }
        }

        private int? _ROLE_ID = -1;
        public int? ROLE_ID
        {
            get { return _ROLE_ID; }
            set { this.RaiseAndSetIfChanged(ref _ROLE_ID, value); }
        }
    }

    public partial class TrainingResult : ReactiveObject
    {

        private int? _ID;
        public int? ID
        {
            get { return _ID; }
            set { this.RaiseAndSetIfChanged(ref _ID, value); }
        }

        private int? _EMP_ID;
        public int? EMP_ID
        {
            get { return _EMP_ID; }
            set { this.RaiseAndSetIfChanged(ref _EMP_ID, value); }
        }

        private int? _DOC_ID;
        public int? DOC_ID
        {
            get { return _DOC_ID; }
            set { this.RaiseAndSetIfChanged(ref _DOC_ID, value); }
        }

        private int? _DOC_LEVEL;
        public int? DOC_LEVEL
        {
            get { return _DOC_LEVEL; }
            set { this.RaiseAndSetIfChanged(ref _DOC_LEVEL, value); }
        }

        private DateTime _VALID_DATE;
        public DateTime VALID_DATE
        {
            get { return _VALID_DATE; }
            set { this.RaiseAndSetIfChanged(ref _VALID_DATE, value); }
        }

        private string _VALID_NOTE;
        public string VALID_NOTE
        {
            get { return _VALID_NOTE; }
            set { this.RaiseAndSetIfChanged(ref _VALID_NOTE, value); }
        }

        private string _EMP_FNAME;
        public string EMP_FNAME
        {
            get { return _EMP_FNAME; }
            set { this.RaiseAndSetIfChanged(ref _EMP_FNAME, value); }
        }

        private string _EMP_LNAME;
        public string EMP_LNAME
        {
            get { return _EMP_LNAME; }
            set { this.RaiseAndSetIfChanged(ref _EMP_LNAME, value); }
        }

        private string _EMP_INITIAL;
        public string EMP_INITIAL
        {
            get { return _EMP_INITIAL; }
            set { this.RaiseAndSetIfChanged(ref _EMP_INITIAL, value); }
        }

        private string _SHIFT_NAME;
        public string SHIFT_NAME
        {
            get { return _SHIFT_NAME; }
            set { this.RaiseAndSetIfChanged(ref _SHIFT_NAME, value); }
        }

        private string _DEPT_NAME;
        public string DEPT_NAME
        {
            get { return _DEPT_NAME; }
            set { this.RaiseAndSetIfChanged(ref _DEPT_NAME, value); }
        }

        private string _VAR_NAME;
        public string VAR_NAME
        {
            get { return _VAR_NAME; }
            set { this.RaiseAndSetIfChanged(ref _VAR_NAME, value); }
        }

        private string _VAR_NAME_TRAINING;
        public string VAR_NAME_TRAINING
        {
            get { return _VAR_NAME_TRAINING; }
            set { this.RaiseAndSetIfChanged(ref _VAR_NAME_TRAINING, value); }
        }

        private string _DOC_CODE;
        public string DOC_CODE
        {
            get { return _DOC_CODE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_CODE, value); }
        }

        private string _DOC_TITLE;
        public string DOC_TITLE
        {
            get { return _DOC_TITLE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_TITLE, value); }
        }

        private int? _DOC_ISSUE;
        public int? DOC_ISSUE
        {
            get { return _DOC_ISSUE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_ISSUE, value); }
        }

        private DateTime _DOC_ISSUEDATE;
        public DateTime DOC_ISSUEDATE
        {
            get { return _DOC_ISSUEDATE; }
            set { this.RaiseAndSetIfChanged(ref _DOC_ISSUEDATE, value); }
        }

        private string _DOC_RA;
        public string DOC_RA
        {
            get { return _DOC_RA; }
            set { this.RaiseAndSetIfChanged(ref _DOC_RA, value); }
        }

        private int? _DOC_CRITERIA;
        public int? DOC_CRITERIA
        {
            get { return _DOC_CRITERIA; }
            set { this.RaiseAndSetIfChanged(ref _DOC_CRITERIA, value); }
        }
    }
}
