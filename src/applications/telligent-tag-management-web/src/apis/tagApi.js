import request from "./request";

export const getTagByCategoryAsync = (categoryId) =>
  request.get(`/api/Tag/summary?categoryid=${categoryId}`);

export const postCreateNewTagAsync = (data) =>
  request.post(`/api/Tag`,data);

export const deleteTagAsync = (tagId) =>
  request.delete(`/api/Tag?id=${tagId}`);