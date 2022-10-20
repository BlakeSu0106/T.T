import request from "./request";

export const getBehaviorTagCategoryAsync = (companyId) =>
  request.get(`/api/BehaviorTagCategory/Activated?companyId=${companyId}`);
  