using Dapper;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WiTrainingSuite.Model
{
    public static class DbInterface
    {
        public static SqlConnection con
        {
            get
            {
                return new SqlConnection(App.ConString);
            }
        }

        // Document Functions

        public static async Task<IEnumerable<DocumentResult>> fnDocumentList()
        {
            return await con.QueryAsync<DocumentResult>("fnDocument_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task<DocumentResult> fnDocumentSelect(DocumentResult document)
        {
            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);

            return await con.QuerySingleAsync<DocumentResult>("fnDocument_Select", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task<IEnumerable<SkillResult>> fnDocumentSelectSkills(DocumentResult document, int _Option)
        {
            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);

            p.Add("@OPTION", _Option);

            return await con.QueryAsync<SkillResult>("fnDocument_Select_Skill", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocumentCreate(DocumentResult document, ReactiveList<SkillResult> skillTags)
        {
            DataTable skillTable = new DataTable();
            skillTable.Columns.Add(new DataColumn("SKILL_ID"));

            foreach (SkillResult s in skillTags)
            {
                DataRow dR = skillTable.NewRow();
                dR["SKILL_ID"] = s.SKILL_ID;
                skillTable.Rows.Add(dR);
            }

            var p = new DynamicParameters();

            p.Add("@DOC_CODE", document.DOC_CODE);
            p.Add("@DOC_TITLE", document.DOC_TITLE);
            p.Add("@DOC_ISSUE", document.DOC_ISSUE);
            p.Add("@DOC_ISSUEDATE", document.DOC_ISSUEDATE);
            p.Add("@DOC_RA", document.DOC_RA);
            p.Add("@DOC_TASK_DESCRIPTION", document.DOC_TASK_DESCRIPTION);
            p.Add("@DOC_RISK_DESCRIPTION", document.DOC_RISK_DESCRIPTION);
            p.Add("@DOCTAG_ID", document.DOCTAG_ID);
            p.Add("@SKILLTAGS", skillTable.AsTableValuedParameter());

            await con.ExecuteAsync("fnDocument_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocumentUpdate(DocumentResult document, ReactiveList<SkillResult> skillTags)
        {
            DataTable skillTable = new DataTable();
            skillTable.Columns.Add(new DataColumn("SKILL_ID"));

            foreach (SkillResult s in skillTags)
            {
                DataRow dR = skillTable.NewRow();
                dR["SKILL_ID"] = s.SKILL_ID;
                skillTable.Rows.Add(dR);
            }

            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);

            p.Add("@DOC_CODE", document.DOC_CODE);
            p.Add("@DOC_TITLE", document.DOC_TITLE);
            p.Add("@DOC_ISSUE", document.DOC_ISSUE);
            p.Add("@DOC_ISSUEDATE", document.DOC_ISSUEDATE);
            p.Add("@DOC_RA", document.DOC_RA);
            p.Add("@DOC_TASK_DESCRIPTION", document.DOC_TASK_DESCRIPTION);
            p.Add("@DOC_RISK_DESCRIPTION", document.DOC_RISK_DESCRIPTION);
            p.Add("@DOCTAG_ID", document.DOCTAG_ID);
            p.Add("@SKILLTAGS", skillTable.AsTableValuedParameter());

            await con.ExecuteAsync("fnDocument_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocumentUpdateNoTraining(DocumentResult document)
        {
            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);

            p.Add("@DOC_CODE", document.DOC_CODE);
            p.Add("@DOC_TITLE", document.DOC_TITLE);
            p.Add("@DOC_ISSUE", document.DOC_ISSUE);
            p.Add("@DOC_ISSUEDATE", document.DOC_ISSUEDATE);
            p.Add("@DOC_RA", document.DOC_RA);
            p.Add("@DOC_TASK_DESCRIPTION", document.DOC_TASK_DESCRIPTION);
            p.Add("@DOC_RISK_DESCRIPTION", document.DOC_RISK_DESCRIPTION);
            p.Add("@DOCTAG_ID", document.DOCTAG_ID);

            await con.ExecuteAsync("fnDocument_Update_No_Training", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocumentDelete(DocumentResult document)
        {
            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);

            await con.ExecuteAsync("fnDocument_Delete", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task<IEnumerable<TrainingResult>> fnDocumentTraining(DocumentResult document)
        {
            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);

            return await con.QueryAsync<TrainingResult>("fnDOCUMENTTRAINING_DOCUMENT_REQUIRED", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocumentTrainingSave(DocumentResult document, ReactiveList<TrainingResult> trainingQueue, DateTime validFromDate)
        {
            DataTable trainingTable = new DataTable();
            trainingTable.Columns.Add("EMP_ID");

            foreach (TrainingResult r in trainingQueue)
            {
                DataRow dR = trainingTable.NewRow();
                dR["EMP_ID"] = r.EMP_ID;
                trainingTable.Rows.Add(dR);
            }

            var p = new DynamicParameters();

            p.Add("@DOC_ID", document.DOC_ID);
            p.Add("@VALID_DATE", validFromDate);
            p.Add("@EMP_QUEUE", trainingTable.AsTableValuedParameter());

            await con.ExecuteAsync("fnDOCUMENTTRAINING_DOCUMENT_SAVE", p, commandType: CommandType.StoredProcedure);
        }

        // Employee Functions

        public static async Task<IEnumerable<EmployeeResult>> fnEmployeeList()
        {
            return await con.QueryAsync<EmployeeResult>("fnEmployee_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task<EmployeeResult> fnEmployeeSelect(EmployeeResult employee)
        {
            var p = new DynamicParameters();

            p.Add("@EMP_ID", employee.EMP_ID);

            return await con.QuerySingleAsync<EmployeeResult>("fnEmployee_Select", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnEmployeeCreate(EmployeeResult employee)
        {
            var p = new DynamicParameters();

            p.Add("@EMP_FNAME", employee.EMP_FNAME);
            p.Add("@EMP_LNAME", employee.EMP_LNAME);
            p.Add("@EMP_INITIAL", employee.EMP_INITIAL);
            p.Add("@EMP_EMAIL", employee.EMP_EMAIL);
            p.Add("@EMP_NOTES", employee.EMP_NOTES);
            p.Add("@EMP_ON_TRAINING_PROGRAMME", employee.EMP_ON_TRAINING_PROGRAMME);
            p.Add("@SHIFT_ID", employee.SHIFT_ID);
            p.Add("@DEPT_ID", employee.DEPT_ID);
            p.Add("@VAR_ID", employee.VAR_ID);
            p.Add("@VAR_ID_TRAINING", employee.VAR_ID_TRAINING);

            await con.ExecuteAsync("fnEmployee_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnEmployeeUpdate(EmployeeResult employee)
        {
            var p = new DynamicParameters();

            p.Add("@EMP_ID", employee.EMP_ID);
            p.Add("@EMP_FNAME", employee.EMP_FNAME);
            p.Add("@EMP_LNAME", employee.EMP_LNAME);
            p.Add("@EMP_INITIAL", employee.EMP_INITIAL);
            p.Add("@EMP_EMAIL", employee.EMP_EMAIL);
            p.Add("@EMP_NOTES", employee.EMP_NOTES);
            p.Add("@EMP_ON_TRAINING_PROGRAMME", employee.EMP_ON_TRAINING_PROGRAMME);
            p.Add("@SHIFT_ID", employee.SHIFT_ID);
            p.Add("@DEPT_ID", employee.DEPT_ID);
            p.Add("@VAR_ID", employee.VAR_ID);
            p.Add("@VAR_ID_TRAINING", employee.VAR_ID_TRAINING);

            await con.ExecuteAsync("fnEmployee_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnEmployeeDelete(EmployeeResult employee)
        {
            var p = new DynamicParameters();

            p.Add("@EMP_ID", employee.EMP_ID);

            await con.ExecuteAsync("fnEmployee_Delete", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task<IEnumerable<TrainingResult>> fnEmployeeTraining(EmployeeResult employee)
        {
            var p = new DynamicParameters();

            p.Add("@EMP_ID", employee.EMP_ID);

            return await con.QueryAsync<TrainingResult>("fnDOCUMENTTRAINING_EMPLOYEE_REQUIRED", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnEmployeeTrainingSave(EmployeeResult employee, ReactiveList<TrainingResult> trainingQueue, DateTime validFromDate)
        {
            DataTable trainingTable = new DataTable();
            trainingTable.Columns.Add("DOC_ID");

            foreach (TrainingResult r in trainingQueue)
            {
                DataRow dR = trainingTable.NewRow();
                dR["DOC_ID"] = r.DOC_ID;
                trainingTable.Rows.Add(dR);
            }

            var p = new DynamicParameters();

            p.Add("@EMP_ID", employee.EMP_ID);
            p.Add("@VALID_DATE", validFromDate);
            p.Add("@DOC_QUEUE", trainingTable.AsTableValuedParameter());

            await con.ExecuteAsync("fnDOCUMENTTRAINING_EMPLOYEE_SAVE", p, commandType: CommandType.StoredProcedure);
        }

        // Employee List Functions

        public static async Task<IEnumerable<DeptResult>> fnDeptList()
        {
            return await con.QueryAsync<DeptResult>("fnDept_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task<IEnumerable<ShiftResult>> fnShiftList()
        {
            return await con.QueryAsync<ShiftResult>("fnShift_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task<IEnumerable<VariantResult>> fnVarList()
        {
            return await con.QueryAsync<VariantResult>("fnVariant_List", commandType: CommandType.StoredProcedure);
        }

        // Department Config

        public static async Task fnDeptCreate(DeptResult dept)
        {
            var p = new DynamicParameters();

            p.Add("@DEPT_NAME", dept.DEPT_NAME);

            await con.ExecuteAsync("fnDept_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDeptUpdate(DeptResult dept)
        {
            var p = new DynamicParameters();

            p.Add("@DEPT_ID", dept.DEPT_ID);
            p.Add("@DEPT_NAME", dept.DEPT_NAME);

            await con.ExecuteAsync("fnDept_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDeptDelete(DeptResult dept)
        {
            var p = new DynamicParameters();

            p.Add("@DEPT_ID", dept.DEPT_ID);

            await con.ExecuteAsync("fnDept_Delete", p, commandType: CommandType.StoredProcedure);
        }

        // Shift Config

        public static async Task fnShiftCreate(ShiftResult shift)
        {
            var p = new DynamicParameters();

            p.Add("@SHIFT_NAME", shift.SHIFT_NAME);

            await con.ExecuteAsync("fnShift_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnShiftUpdate(ShiftResult shift)
        {
            var p = new DynamicParameters();

            p.Add("@SHIFT_ID", shift.SHIFT_ID);
            p.Add("@SHIFT_NAME", shift.SHIFT_NAME);

            await con.ExecuteAsync("fnShift_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnShiftDelete(ShiftResult shift)
        {
            var p = new DynamicParameters();

            p.Add("@SHIFT_ID", shift.SHIFT_ID);

            await con.ExecuteAsync("fnShift_Delete", p, commandType: CommandType.StoredProcedure);
        }

        // Skill Config

        public static async Task<IEnumerable<SkillResult>> fnSkillList()
        {
            return await con.QueryAsync<SkillResult>("fnSkill_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task fnSkillCreate(SkillResult skill)
        {
            var p = new DynamicParameters();

            p.Add("@SKILL_NAME", skill.SKILL_NAME);

            await con.ExecuteAsync("fnSkill_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnSkillUpdate(SkillResult skill)
        {
            var p = new DynamicParameters();

            p.Add("@SKILL_ID", skill.SKILL_ID);
            p.Add("@SKILL_NAME", skill.SKILL_NAME);

            await con.ExecuteAsync("fnSkill_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnSkillDelete(SkillResult skill)
        {
            var p = new DynamicParameters();

            p.Add("@SKILL_ID", skill.SKILL_ID);

            await con.ExecuteAsync("fnSkill_Delete", p, commandType: CommandType.StoredProcedure);
        }

        // Role Config

        public static async Task<IEnumerable<RoleResult>> fnRoleList()
        {
            return await con.QueryAsync<RoleResult>("fnRole_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task fnRoleCreate(RoleResult role)
        {
            var p = new DynamicParameters();

            p.Add("@ROLE_NAME", role.ROLE_NAME);
            p.Add("@ROLE_HAS_EMAIL", role.ROLE_HAS_EMAIL);

            await con.ExecuteAsync("fnRole_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnRoleUpdate(RoleResult role)
        {
            var p = new DynamicParameters();

            p.Add("@ROLE_ID", role.ROLE_ID);
            p.Add("@ROLE_NAME", role.ROLE_NAME);
            p.Add("@ROLE_HAS_EMAIL", role.ROLE_HAS_EMAIL);

            await con.ExecuteAsync("fnRole_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnRoleDelete(RoleResult role)
        {
            var p = new DynamicParameters();

            p.Add("@ROLE_ID", role.ROLE_ID);

            await con.ExecuteAsync("fnRole_Delete", p, commandType: CommandType.StoredProcedure);
        }

        // Document Tag Config

        public static async Task<IEnumerable<DocTagResult>> fnDocTagList()
        {
            return await con.QueryAsync<DocTagResult>("fnDocTag_List", commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocTagCreate(DocTagResult doc)
        {
            var p = new DynamicParameters();

            p.Add("@DOCTAG_NAME", doc.DOCTAG_NAME);

            await con.ExecuteAsync("fnDocTag_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocTagUpdate(DocTagResult doc)
        {
            var p = new DynamicParameters();

            p.Add("@DOCTAG_ID", doc.DOCTAG_ID);
            p.Add("@DOCTAG_NAME", doc.DOCTAG_NAME);

            await con.ExecuteAsync("fnDocTag_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnDocTagDelete(DocTagResult doc)
        {
            var p = new DynamicParameters();

            p.Add("@DOCTAG_ID", doc.DOCTAG_ID);

            await con.ExecuteAsync("fnDocTag_Delete", p, commandType: CommandType.StoredProcedure);
        }

        // Variant Config

        public static async Task fnVarCreate(VariantResult variant)
        {
            var p = new DynamicParameters();

            p.Add("@VAR_NAME", variant.VAR_NAME);
            p.Add("@ROLE_ID", variant.ROLE_ID);

            await con.ExecuteAsync("fnVariant_Create", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnVarUpdate(VariantResult variant)
        {
            var p = new DynamicParameters();

            p.Add("@VAR_ID", variant.VAR_ID);
            p.Add("@VAR_NAME", variant.VAR_NAME);
            p.Add("@ROLE_ID", variant.ROLE_ID);

            await con.ExecuteAsync("fnVariant_Update", p, commandType: CommandType.StoredProcedure);
        }

        public static async Task fnVarDelete(VariantResult variant)
        {
            var p = new DynamicParameters();

            p.Add("@VAR_ID", variant.VAR_ID);

            await con.ExecuteAsync("fnVariant_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
