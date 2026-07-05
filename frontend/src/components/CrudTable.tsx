import { useState } from "react";
import { Button, DatePicker, Form, Input, Modal, Popconfirm, Select, Space, Switch, Table, App } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { Dayjs } from "dayjs";
import type { PagedResult } from "../types";

export interface CrudFormField {
  name: string;
  label: string;
  type?: "text" | "number" | "password" | "select" | "switch" | "email" | "date";
  required?: boolean;
  options?: { label: string; value: number | string }[];
  hideOnEdit?: boolean;
  hideOnCreate?: boolean;
}

interface CrudTableProps<T extends { id: number }, TCreate, TUpdate> {
  title: string;
  queryKey: string;
  fetchPaged: (params: Record<string, unknown>) => Promise<PagedResult<T>>;
  columns: ColumnsType<T>;
  formFields: CrudFormField[];
  toFormValues?: (record: T) => Record<string, unknown>;
  onCreate?: (dto: TCreate) => Promise<unknown>;
  onUpdate?: (id: number, dto: TUpdate) => Promise<unknown>;
  onDelete?: (id: number) => Promise<unknown>;
  canCreate?: boolean;
  canEdit?: boolean;
  canDelete?: boolean;
  searchPlaceholder?: string;
  extraParams?: Record<string, unknown>;
}

export function CrudTable<T extends { id: number }, TCreate, TUpdate>({
  title,
  queryKey,
  fetchPaged,
  columns,
  formFields,
  toFormValues,
  onCreate,
  onUpdate,
  onDelete,
  canCreate,
  canEdit,
  canDelete,
  searchPlaceholder,
  extraParams,
}: CrudTableProps<T, TCreate, TUpdate>) {
  const { message, modal } = App.useApp();
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [keyword, setKeyword] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<T | null>(null);
  const [form] = Form.useForm();

  const params = { page, pageSize, keyword: keyword || undefined, ...extraParams };

  const { data, isLoading } = useQuery({
    queryKey: [queryKey, params],
    queryFn: () => fetchPaged(params),
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: [queryKey] });

  const createMutation = useMutation({
    mutationFn: (dto: TCreate) => onCreate!(dto),
    onSuccess: () => {
      message.success("Tạo mới thành công");
      setModalOpen(false);
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: TUpdate }) => onUpdate!(id, dto),
    onSuccess: () => {
      message.success("Cập nhật thành công");
      setModalOpen(false);
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => onDelete!(id),
    onSuccess: () => {
      message.success("Xoá thành công");
      invalidate();
    },
    onError: (err: any) => {
      const errors: string[] | undefined = err?.response?.data?.errors;
      const errorMessage = err?.response?.data?.message || "Có lỗi xảy ra";
      if (errors && errors.length > 0) {
        modal.error({
          title: errorMessage,
          content: (
            <ul style={{ paddingLeft: 20, margin: 0 }}>
              {errors.map((e, i) => (
                <li key={i}>{e}</li>
              ))}
            </ul>
          ),
        });
      } else {
        message.error(errorMessage);
      }
    },
  });

  const openCreate = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEdit = (record: T) => {
    setEditingRecord(record);
    form.setFieldsValue(toFormValues ? toFormValues(record) : record);
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    // DatePicker trả về đối tượng Dayjs — API cần chuỗi ISO "YYYY-MM-DD".
    for (const field of formFields) {
      if (field.type === "date" && values[field.name]) {
        values[field.name] = (values[field.name] as Dayjs).format("YYYY-MM-DD");
      }
    }
    if (editingRecord) {
      updateMutation.mutate({ id: editingRecord.id, dto: values as TUpdate });
    } else {
      createMutation.mutate(values as TCreate);
    }
  };

  const tableColumns: ColumnsType<T> = [
    ...columns,
    ...(canEdit || canDelete
      ? [
          {
            title: "Hành động",
            key: "actions",
            render: (_: unknown, record: T) => (
              <Space>
                {canEdit && (
                  <Button size="small" onClick={() => openEdit(record)}>
                    Sửa
                  </Button>
                )}
                {canDelete && (
                  <Popconfirm
                    title="Xác nhận xoá?"
                    onConfirm={() => deleteMutation.mutate(record.id)}
                  >
                    <Button size="small" danger>
                      Xoá
                    </Button>
                  </Popconfirm>
                )}
              </Space>
            ),
          },
        ]
      : []),
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16, width: "100%", justifyContent: "space-between" }}>
        <h2 style={{ margin: 0 }}>{title}</h2>
        <Space>
          {searchPlaceholder && (
            <Input.Search
              placeholder={searchPlaceholder}
              allowClear
              onSearch={(v) => {
                setKeyword(v);
                setPage(1);
              }}
              style={{ width: 240 }}
            />
          )}
          {canCreate && (
            <Button type="primary" onClick={openCreate}>
              Thêm mới
            </Button>
          )}
        </Space>
      </Space>

      <Table
        rowKey="id"
        loading={isLoading}
        columns={tableColumns}
        dataSource={data?.data || []}
        pagination={{
          current: page,
          pageSize,
          total: data?.total || 0,
          onChange: (p, ps) => {
            setPage(p);
            setPageSize(ps);
          },
          showSizeChanger: true,
        }}
      />

      <Modal
        title={editingRecord ? `Sửa ${title}` : `Thêm mới ${title}`}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onOk={handleSubmit}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        destroyOnHidden
      >
        <Form form={form} layout="vertical">
          {formFields
            .filter((f) => (editingRecord ? !f.hideOnEdit : !f.hideOnCreate))
            .map((field) => (
              <Form.Item
                key={field.name}
                name={field.name}
                label={field.label}
                rules={field.required ? [{ required: true, message: `Vui lòng nhập ${field.label}` }] : []}
                valuePropName={field.type === "switch" ? "checked" : "value"}
              >
                {renderField(field)}
              </Form.Item>
            ))}
        </Form>
      </Modal>
    </div>
  );
}

function renderField(field: CrudFormField) {
  switch (field.type) {
    case "number":
      return <Input type="number" />;
    case "password":
      return <Input.Password />;
    case "switch":
      return <SwitchField />;
    case "select":
      return <SelectField options={field.options || []} />;
    case "date":
      return <DatePicker style={{ width: "100%" }} format="DD/MM/YYYY" />;
    default:
      return <Input />;
  }
}

function SwitchField(props: any) {
  return <Switch {...props} />;
}

function SelectField({ options, ...rest }: { options: { label: string; value: number | string }[] } & Record<string, unknown>) {
  return <Select options={options} {...rest} />;
}
