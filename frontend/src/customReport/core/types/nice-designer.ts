export interface ComponentSchema {
    id?: string;
    type: string;
    label?: string;
    componentProps?: any;
    children?: ComponentSchema[];
}
