import request from "./request";

export const getBatchTransactionLogAsync = () =>
  request.get('/api/BatchTransactionLog/All');
  