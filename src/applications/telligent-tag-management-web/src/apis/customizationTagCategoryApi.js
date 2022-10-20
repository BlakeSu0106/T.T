import request from "./request";

export const getCustomizationTagCategoryAsync = (companyId) =>
  request.get(`/api/BehaviorTagCategory/Activated?companyId=${companyId}`);

export const getAllCustomizationTagCategoryAsync = (companyId) =>
  request.get(`/api/CustomizationTagCategory/All?companyId=${companyId}`);