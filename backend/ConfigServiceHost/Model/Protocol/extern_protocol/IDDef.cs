namespace XViewer.Model.Data
{
    //定义与服务器通信中各命令字和字段的ID

    //所有命令枚举，共256个
    public enum Cmds : byte
    {
        ApplyUID = 0, //申请唯一标识
        ReturnUID,//服务器返回唯一标识
        QueryConfig, //查询配置
        ReturnConfig,//返回配置
        SavePersonalConfig,//保存配置
        SaveConfigRsp,//保存配置状态:5
        QueryExam,//查询检查列表
        ReturnExam,//返回检查列表
        BuildExam,//查询检查骨架信息
        ReturnBuildExam,//返回检查骨架信息
        LoadExam,//下载指定检查:10
        ReturnLoadExam,//返回整个检查
        LoadSeries,//下载指定序列
        ReturnLoadSeries,//返回指定序列
        LoadImage,//下载指定图像
        ReturnLoadImage,//返回指定图像:15
        QueryDocument,				//查询文档列表
        QueryDocumentResponse,		//返回文档列表
        LoadDocumentSkeleton,       //下载区域下的骨架信息
        LoadDocumentSkeletonResponse,	// 返回文档包含的骨架信息
        LoadDocument,				//下载指定文档:20        
        LoadDocumentResponse,		//返回整个文档
        ImageSendFinish,//服务器告知客户端图像发送完毕
        //以下定义是为了和appserver上的定义保持一致而增加，终端并不需要
        AddPatient,            //新增病人
        UpdatePatient,         //更新病人
        MergePatient,          //合并病人:25
        SubmitDoc,             //提交文档
        SubmitDocResponse,        //提交文档的回复消息
        //结束
        SaveLog,//保存日志
        RisViewRequest,//RIS阅片请求
        ApplyAuthority,//申请权限:30
        ApplyAuthorityResponse,//服务器返回
        ReportThrob,//报告心跳
        ReleaseAuthority,//释放权限
        LoadRegionImage,//优先下载区域下的图像：请求
        ReturnLoadRegionImage,//优先下载区域下的图像：返回包:35
        LoadGeneralConfig,//下载通用配置
        ExportImage,//导出图像
        ExportImageResponse,//导出图像是否成功的回包
        QueryVersion,//查询版本号
        QueryVersionResponse,//查询版本号的返回包 40
        QueryPatientInfo,//查询病人信息
        QueryPatientInfoResponse,//查询病人信息返回包
        MatchImage,//匹配
        MatchImageResponse,//匹配请求的返回包
        DeMatchImage,//取消匹配 45
        DeMatchImageResponde,//取消匹配的返回包
        ApplyResource,//申请资源请求
        ApplyResourceResponse,//申请资源请求返回包
        DicomVerify,//DICOM VERIFY
        DicomVerifyResponse,//verify返回状态 50
        DicomQuery,//DICOM QUERY
        DicomQueryResponse,
        LoadDicomSkeleton,//Q/R时下载DICOM骨架信息
        LoadDicomSkeletonResponse,//QR时下载DICOM骨架信息的返回包
        RetrieveDicomExam, //获取DICOM检查 55
        RetrieveDicomExamResponse,//获取DICOM检查返回包
        DicomPrint,
        DicomPrintResponse,
        LoadGeneralConfigResponse,//加载通用配置返回包
        LoadPersonalConfig, //加载个性化配置请求		60
        LoadPersonalConfigResponse,//加载个性化配置返回包        
        AddPatientResponse,            //新增病人返回
        UpdatePatientResponse,         //更新病人返回
        MergePatientRessponse,          //合并病人返回
        LoadTerminalConfig,           //下载终端相关的配置 65
        LoadTerminalConfigResponse,
        LoadConfigServiceParameters,//获取配置服务参数
        LoadConfigServiceParametersResponse,//获取配置服务的返回包
        SaveTerminalConfig, //保存终端配置
        PeriodicalLoadStudy,// 70
        PeriodicalLoadStudyResponse,
        ApplyMPRAuthority, //3D MPR权限
        ReleaseMPRAuthority, //释放3D MPR权限
        ApplyVRAuthority, //3D VR权限
        ReleaseVRAuthority, //释放3D VR权限 75
        RisPreViewRequest, //来自RIS的预加载请求
        RisPreViewRequestResponse,//来自RIS的预加载请求的响应
        SymmetricConsultQueryStudyInfoRequest,//同步会诊检查信息获取请求
        QueryHistoryStudyRequest,//查询历史检查请求
        QueryHistroryStudyResponse,//查询历史检查响应 80
        UserQuery,		//用户查询
        UserqueryResponse,	//用户查询反馈
        ChangePassword,//密码修改
        ConsultationCorrectRequest,//同步会诊校准消息
        SymmetricConsultRequestBegin,
        SymmetricConsultRequestFinished,
        UpLoadMergeExam,//上传PDU，合并检查 87
        UpLoadDeleteExam,//上传PDU，删除检查
        UpLoadResponse,//上传PDU，回复消息
        COMMAND_UPDATE_COMPLETE,//PDU发送合并检查结果消息
        LoadAuthorityConfig,//下载权限配置
        LoadAuthorityConfigResponse,//加载权限配置返回包 
        SaveAuthorityConfig,//保存配置
        DicomStorage, // Dicom存储
        DicomStorageResponce, // Dicom存储回应
        AddImageEx, // 增加图像扩展信息
        DeleteImageEx, // 删除图像扩展信息
        COMMAND_DICOM_CONVERT,            //DICOM 转换为AVI         98  新增的
        COMMAND_DICOM_CONVERT_RESPONSE,   // DICOM转换为AVI回复     99  新增的
        COMMAND_UPDATE_EXAM, // 上传PDU, 更新检查信息
        COMMAND_EXTEND_PRINT,// 从外部(xplayer)发起打印一张图像的请求
        COMMAND_DICOM_PARSE,
        COMMAND_DICOM_PARSE_RESPONSE,
        WebApplyAuthority, //网页版授权
        WebReleaseAuthority,//释放权限
        MobileApplyAuthority, //移动版授权
        MobileReleaseAuthority,//释放权限
        COMMAND_DICOM_PARSE_STRUCT,                 // 解析一组DICOM文件的骨架信息     
        COMMAND_DICOM_PARSE_STRUCT_RESPONSE,        // 解析一组DICOM文件的骨架信息回复
        RisApplyAuthority,//Ris授权，目前仅是获取配置IP端口，不进行实际的授权管理
        RisReleaseAuthority,//Release Ris授权，目前没实际授权，因此暂不使用
        LoadDepartmentConfigResponse,//加载部门配置响应     
        LoadDepartmentConfig, //加载部门配置请求
        SaveDepartmentConfig,//保存配置
    }

    // 旧的参数定义,和ris通信使用
    public enum ParamsOld
    {
        Cmd = 0,
        AccessionNum = 6,
        ExamDate = 14,
        ExamStatus = 18,
        Modality = 47,
        StudySeqBegin = 129,
        SequenceEnd = 142,
        ItemEnd = 144,
        PacketLen = 146,
        ViewParams = 147,
        Region = 171,
    }

    //所有参数枚举
    public enum Params
    {
        PARAMETER_UNKNOWN = -1,

        PARAMETER_SEQUENCE_START = 0,
        PARAMETER_SEQUENCE_STUDY,
        PARAMETER_SEQUENCE_SERIES,
        PARAMETER_SEQUENCE_IMAGE,
        PARAMETER_SEQUENCE_MODALITY,
        PARAMETER_SEQUENCE_PATID,
        PARAMETER_SEQUENCE_ACCNO,
        PARAMETER_SEQUENCE_HISORDERID,
        PARAMETER_SEQUENCE_SEX,
        PARAMETER_SEQUENCE_DOCUMENT,
        PARAMETER_SEQUENCE_DOCTYPE,
        PARAMETER_SEQUENCE_EXAMTYPE,
        PARAMETER_SEQUENCE_AREA,//查询区域序列，有时候需要同时查询多个区域，如主院区和分院区需要同时查询
        PARAMETER_SEQUENCE_OVERLAY,//overlay序列，一张图可能包含多个overlay,每个overlay对应sequence下的一个item
        PARAMETER_SEQUENCE_PATIENTKEY,
        PARAMETER_SEQUENCE_RefImage,
        PARAMETER_SEQUENCE_DOCKEY,
        PARAMETER_SEQUENCE_USERNAME,				//用户列表
        PARAMETER_SEQUENCE_SOURCE,      //上传PDU，合并检查的源检查
        PARAMETER_SEQUENCE_DESTINATION, //上传PDU，合并检查的目的检查
        PARAMETER_SEQUENCE_PR,
        PARAMETER_SEQUENCE_HISORDERSTATUS,               // 检查状态seq
        PARAMETER_SEQUENCE_CornerInfo, //四角信息序列
        PARAMETER_SEQUENCE_UPDATE, //更新项目序列
        PARAMETER_SEQUENCE_END = 9999,


        PARAMETER_COMMANDID = 10000,//标识请求
        PARAMETER_UNUSED,
        PARAMETER_TERMINALID,//
        PARAMETER_STUDY_ID,//检查ID
        PARAMETER_PATIENT_ID,// 2013_05_23 新加字段，终端已经解析
        PARAMETER_PATIENT_NAME, //:
        PARAMETER_ACCESSIONNUM,// 2013_05_23 新加字段，终端已经解析
        PARAMETER_PATIENT_SEX, // 2013_05_23 新加字段，终端已经解析
        PARAMETER_PATIENT_AGE, // 2013_05_23 新加字段
        PARAMETER_PatientOrientation,//四角信息用
        PARAMETER_ViewPosition,//四角信息用:
        PARAMETER_STUDY_DESCRIPTION,// 2013_05_23 新加字段，终端已经解析
        PARAMETER_STUDY_DATEFROM,
        PARAMETER_STUDY_DATETO,
        PARAMETER_STUDY_DATE, // 2013_05_23 新加字段，终端已经解析
        PARAMETER_STUDY_TIME, // 2013_05_23 新加字段:
        PARAMETER_ExamBodyPart,
        PARAMETER_ExamName,
        PARAMETER_ExamStatus,
        PARAMETER_ExamRequest,
        PARAMETER_ExamOffice,//
        PARAMETER_InPatientSection,
        PARAMETER_BedID,
        PARAMETER_VerifyStatus,
        PARAMETER_PatientType,
        PARAMETER_Patient_DOB, // 2013_05_23 新加字段:
        PARAMETER_MachineRoom,
        PARAMETER_BodyPartExamined,// 2013_05_23 新加字段，终端已经解析
        PARAMETER_MachineID,
        PARAMETER_MatchStatus,
        PARAMETER_Manufacturer, // 2013_05_23 新加字段:
        PARAMETER_ManufacturerModelName, // 2013_05_23 新加字段
        PARAMETER_InstitutionName, // 2013_05_23 新加字段
        PARAMETER_RefferringPhysicianName,// 2013_05_23 新加字段
        PARAMETER_StationName,// 2013_05_23 新加字段
        PARAMETER_OperatorName,// 2013_05_23 新加字段:
        PARAMETER_Contrast,// 2013_05_23 新加字段
        PARAMETER_StudyComment,// 2013_05_23 新加字段
        PARAMETER_ReportID,
        PARAMETER_OrderID,
        PARAMETER_PATIENT_BIRTHDAY,//
        PARAMETER_PATIENT_ADDRESS,// 2013_05_23 新加字段
        PARAMETER_STUDY_UID,
        PARAMETER_SERIES_UID,
        PARAMETER_SERIES_ID,
        PARAMETER_SERIESCOUNT,//
        PARAMETER_IMAGECOUNT,
        PARAMETER_MODALITY,
        PARAMETER_SERIES_INDEX,
        PARAMETER_SERIES_DESCRIPTION,// 2013_05_23 新加字段，终端已经解析
        PARAMETER_PatientPosition,// 2013_05_23 新加字段,是否与Image Postion重复？
        PARAMETER_SeriesTime,//2013_05_23 新加字段
        PARAMETER_SeriesNumber,//2013_05_23 新加字段
        PARAMETER_IMAGE_UID,
        PARAMETER_IMAGE_ID,// 2013_05_23 新加字段，终端已经解析
        PARAMETER_IMAGE_TYPE,// 2013_05_23 新加字段，终端已经解析:
        PARAMETER_IMAGE_INDEX,
        PARAMETER_IMAGE_Width,
        PARAMETER_IMAGE_Height,
        PARAMETER_IMAGE_WindowWidth,
        PARAMETER_IMAGE_WindowLevel,//
        PARAMETER_IMAGE_BITS,
        PARAMETER_IMAGE_Intercept,
        PARAMETER_IMAGE_Slope,
        PARAMETER_IMAGE_SliceSpace,
        PARAMETER_IMAGE_SliceThickness,// 2013_05_23 新加字段，终端已经解析:
        PARAMETER_IMAGE_SliceLocation,//去掉以前的ImageLocation定义，用slicelocation取代之
        PARAMETER_IMAGE_Photometric,
        PARAMETER_IMAGE_HighBit,
        PARAMETER_IMAGE_BitsAllocated,
        PARAMETER_IMAGE_BitsStored,//
        PARAMETER_IMAGE_Representation,//DICOM里的元素
        PARAMETER_IMAGE_Min,
        PARAMETER_IMAGE_Max,
        PARAMETER_IMAGE_AbsMin,
        PARAMETER_IMAGE_AbsMax,        //
        PARAMETER_IMAGE_AbsRange,
        PARAMETER_IMAGE_LowPixelValue,
        PARAMETER_IMAGE_HighPixelValue,
        PARAMETER_IMAGE_ValuePresentation,//经过运算得到的结果
        PARAMETER_IMAGE_SamplePerPixel,//
        PARAMETER_IMAGE_Position,
        PARAMETER_IMAGE_Orient,
        PARAMETER_IMAGE_PixSPace,
        PARAMETER_IMAGE_PixelSPacing,
        PARAMETER_IMAGE_PixelAspectRatio,//
        PARAMETER_IMAGE_FrameRefUID,
        PARAMETER_IMAGE_SOPClassUID,
        PARAMETER_IMAGE_RefSOPInstanceUID,
        PARAMETER_IMAGE_AcquisitionDate, //2013_05_23 新加字段:
        PARAMETER_IMAGE_AcquisitionTime,// 2013_05_23 新加字段
        PARAMETER_IMAGE_ScanningSequence,// 2013_05_23 新加字段
        PARAMETER_IMAGE_Radiopharmaceutical,// 2013_05_23 新加字段
        PARAMETER_IMAGE_KVP,// 2013_05_23 新加字段
        PARAMETER_IMAGE_CountsAccumulated,// 2013_05_23 新加字段:
        PARAMETER_IMAGE_RepetitionTime,// 2013_05_23 新加字段
        PARAMETER_IMAGE_EchoTime,// 2013_05_23 新加字段
        PARAMETER_IMAGE_MagneticFieldStrength,// 2013_05_23 新加字段
        PARAMETER_IMAGE_SpacingBetweenSlices,// 2013_05_23 新加字段
        PARAMETER_IMAGE_EchoTrainLength,// 2013_05_23 新加字段:
        PARAMETER_IMAGE_ReconstructionDiameter,// 2013_05_23 新加字段
        PARAMETER_IMAGE_GantryDetectorTilt,// 2013_05_23 新加字段
        PARAMETER_IMAGE_ExposureTime,// 2013_05_23 新加字段
        PARAMETER_IMAGE_XrayTubeCurrent,// 2013_05_23 新加字段
        PARAMETER_IMAGE_Exposure,// 2013_05_23 新加字段:
        PARAMETER_IMAGE_ConvolutionKernel,// 2013_05_23 新加字段
        PARAMETER_IMAGE_ActualFrameDuration,// 2013_05_23 新加字段
        PARAMETER_IMAGE_ReceivingCoil,// 2013_05_23 新加字段    
        PARAMETER_IMAGE_TableSpeed,// 2013_05_23 新加字段  
        PARAMETER_IMAGE_GantryPeriod,// 2013_05_23 新加字段:
        PARAMETER_IMAGE_InstanceNumber,// 2013_05_23 新加字段 是否与原来添加的Image Number重复？
        PARAMETER_Laterality,// 2013_05_23 新加字段 
        PARAMETER_IMAGE_ImageComment,// 2013_05_23 新加字段
        PARAMETER_IMAGE_ScanPitchRatio,// 2013_05_23 新加字段
        PARAMETER_IMAGE_EnergyWindowName,// 2013_05_23 新加字段:
        PARAMETER_IMAGE_PIXEL,
        PARAMETER_IMAGE_VALIDSTATUS,//标识图像是否有效，有些图像服务器会解析失败
        PARAMETER_IMAGE_COMPRESSTYPE,//图像压缩类型
        PARAMETER_DOCUMENT_UID,//文档唯一id
        PARAMETER_DOCUMENT_REPOSITORYUID,//文档源id:
        PARAMETER_DOCUMENT_CREATIONTIME,//文档创建日期和事件
        PARAMETER_DOCUMENT_TYPE,//文档类型：KOS or PDF
        PARAMETER_DOCUMENT_DATEFROM,//查询时间范围，即将废除，复用examdatefrom
        PARAMETER_DOCUMENT_DATETO,//查询时间范围，即将废除，复用examdateto
        PARAMETER_DOCUMENT_PATIENTID,//源病人ID:
        PARAMETER_DOCUMENT_ORGANIZATION,//病人源ID对应的医疗机构
        PARAMETER_DOCUMENT_EXAM_TYPE,
        PARAMETER_DOCUMENT_EXAM_ITEM,
        PARAMETER_ITEM,
        PARAMETER_ITEM_END,
        PARAMETER_LOG,//
        PARAMETER_PACKAGE_LENGTH,//包长度   
        PARAMETER_ViewParams,//阅片参数
        PARAMETER_HisOrderID,//HIS的检查ID,
        //以下定义为了和appserver保持一致
        PARAMETER_PACKAGE,
        PARAMETER_PATIENTKEY,//
        PARAMETER_OLDPATIENTKEY,
        PARAMETER_DOCKEY,
        PARAMETER_RESPONSE_STATUS,//
        PARAMETER_RESPONSE_ERRORINFO,//服务器返回的失败信息
        PARAMETER_THROBINTERVAL,//授权服务器返回给客户端指定的心跳频率
        PARAMETER_LOGINFO_LEVEL,//日志等级
        PARAMETER_LOGINFO_HOSPITALNAME,//医院名称
        PARAMETER_LOGINFO_DEPARTMENTID,//科室编号//
        PARAMETER_LOGINFO_DEPARTMENTNAME,//科室名称
        PARAMETER_LOGINFO_DOCTORID,//医生工号
        PARAMETER_LOGINFO_DOCTORNAME,//医生姓名
        PARAMETER_LOGINFO_PATIENTID,//病人ID
        PARAMETER_LOGINFO_PATIENTNAME,//病人姓名//
        PARAMETER_LOGINFO_PATIENTSEX,//病人性别
        PARAMETER_LOGINFO_PATIENTAC,//检查号
        PARAMETER_ACCOUNT,//用户账号
        PARAMETER_PASSWORD,//用户密码    
        PARAMETER_CONFIG,//配置：包括通用配置和个性化配置
        PARAMETER_AREA_ID,//区域描述：比如说主院区和分院区
        PARAMETER_OVERLAY_ROW,
        PARAMETER_OVERLAY_COLUM,
        PARAMETER_OVERLAY_TYPE,
        PARAMETER_OVERLAY_ORIGIN_LEFT,//
        PARAMETER_OVERLAY_ORIGIN_TOP,
        PARAMETER_OVERLAY_BITSALLOC,
        PARAMETER_OVERLAY_BITPOSITION,
        PARAMETER_OVERLAY_SUBTYPE,
        PARAMETER_OVERLAY_LABEL,//
        PARAMETER_OVERLAY_DATA,
        PARAMETER_FILE_PATH,//文件路径
        PARAMETER_FILE_NAME,//文件名
        PARAMETER_FILE_VERSION,//版本号
        PARAMETER_ExportStatus,//文件导出成功与否的状态
        PARAMETER_Port,//端口号
        PARAMETER_CornerInfo,//四角信息 
        PARAMETER_CornerInfoPos,//四角信息位置
        PARAMETER_LMOrientation,//左中体位标识
        PARAMETER_BMOrientation,//下中体位标识
        PARAMETER_ScaleHeight,//比例尺高度
        PARAMETER_ScaleUnit,//比例尺单位
        PARAMETER_ScaleValid,//标识比例尺是否有效
        PARAMETER_QueryResult_Intactness,//查询结果是否完整
        PARAMETER_QUERY_QUALIFIER,//查询限定标识
        PARAMETER_SCALE_RATIO,//缩放系数        
        PARAMETER_CHECKIN_DATE,//登记日期
        PARAMETER_CHECKIN_TIME,//登记时间         
        PARAMETER_MATCHACTION_RESULT,//请求执行情况
        PARAMETER_IP,//IP地址
        PARAMETER_SERVICECOUNT,//服务数量，指服务器端能够提供的某一类服务如PDV服务的个数，有时候服务器端会同时提供多个相同的服务，以作负载均衡
        PARAMETER_AETITLE,//
        PARAMETER_PRINT_CopiesNumber,               //打印份数
        PARAMETER_PRINT_PrintPriority,              //打印优先级,
        PARAMETER_PRINT_MediumType,                 //打印介质类型
        PARAMETER_PRINT_FilmDestination,            //打印目标
        PARAMETER_PRINT_FilmSessionLabel,           //Film Session标签
        PARAMETER_PRINT_ImageDisplayFormat,         //打印显示格式
        PARAMETER_PRINT_PhotometricInterpretion,    //打印颜色,
        PARAMETER_PRINT_FilmOrientation,            //胶片方向
        PARAMETER_PRINT_FilmSizeID,                 //胶片大小
        PARAMETER_PRINT_MagnificationType,          //缩放类型
        PARAMETER_PRINT_BorderDensity,              //间隔样式
        PARAMETER_PRINT_EmptyImageDensity,          //空图像样式,
        PARAMETER_PRINT_MaxDensity,                 //打印最大密度
        PARAMETER_PRINT_MinDensity,                 //打印最小密度
        PARAMETER_PRINT_Trim,                       //整理打印
        PARAMETER_PRINT_ConfigurationInfo,          //打印配置信息
        PARAMETER_PATIENT_OtherID,                  //病人其它ID，
        PARAMETER_PATIENT_NAME_ENG,                  //病人英文名
        PARAMETER_PATIENT_PhoneNumber,              //病人电话号码
        PARAMETER_PATIENT_UpdateStatus,             //病人更新标识
        PARAMETER_STUDYID_DICOM,                    //DICOM STUDY ID
        PARAMETER_QueryMode,                       //查询模式，告诉服务器是否要精确查询 
        PARAMETER_HospitalID,                      //医院ID
        PARAMETER_DICOM_IMAGETYPE,                  //DICOM里的image type字段                
        PARAMETER_ViewerAuthorityID,               //Viewer授权模块ID
        PARAMETER_MPRAuthorityID,                  //MPR授权模块ID
        PARAMETER_VRAuthorityID,                   //VR授权模块ID 
        PARAMETER_IMAGE_Laterality,                 //
        PARAMETER_IMAGE_LOCALCACHEPATH,                   //PDV上存放缓存文件的路径
        PARAMETER_PRINT_AdditionInfo,               //打印附加信息
        PARAMETER_ModalityId,                      //模态号，用于区别病人在某台设备做的检查序号
        PARAMETER_InPatientId,                     //住院号 
        PARAMETER_OutPatientId,                    //门诊号         
        PARAMETER_USERCODE,						 //用户代码
        PARAMETER_USERQUERY_FUNCTIONCODE,           //用户查询相关细分功能
        PARAMETER_NEEDCHECKACCOUNTFLAG,            //授权服务是否检查账号有效标志 
        PARAMETER_REQUEST_ID,
        PARAMETER_CharacterSet,          //字符集
        PARAMETER_LOAD_CONFIG_TYPE,		 //请求的配置类型，如个人默认配置、个人配置、终端默认配置，终端配置等
        PARAMETER_SeriesDate,
        PARAMETER_IMAGEEX_ID,
        PARAMETER_IMAGEEX_TIME,
        PARAMETER_IMAGEEX_CONTENT,                  //
        PARAMETER_USER_ID,
        PARAMETER_IMAGEEX_DESCRIPTION,
        PARAMETER_AVI_FILE_PATH,         //avi路径
        PARAMETER_NEEDCHECKPWDFLAG,           //授权服务是否检查密码有效标志
        PARAMETER_REPEAT_INDEX,                     //重复序号
        PARAMETER_HISORDERSTATUS,                  //检查状态
        PARAMETER_Variant,                  //Variant of the Scanning Sequence
        PARAMETER_ACCREDIT,                        //图像查询是否受限制的,0表示受限制
        PARAMETER_Record,                          //查询检查列表返回结果
        PARAMETER_BackupIndex,                     //图像备份index
        PARAMETER_PRINTCOUNT,                       // 已打印次数
        PARAMETER_PRINTDT,                          // 打印时间
        PARAMETER_NumberOfFrames,                   //dcm多帧帧数   10241
        PARAMETER_ClientType,                     //客户端类型
        PARAMETER_Department,                     //部门类型
    }

    //图像类型枚举                
    public enum ImageType : byte
    {
        StudyIcon,//0
        SeriesIcon,
        BMP,
        Gray,
        SR,
        KOSFILE,//5
        PDFFile,
        PDFURI,
        AVI,
        JPG,
        DCM,//10
        DCMMULTIFRAME,//多帧DICOM文件
        Unknown,
    }

    //图像是否有效标识
    enum ImageValid : byte
    {
        Invalid = 0,
        Valid,
    }

    //打印是否成功标识
    enum PrintResponce : byte
    {
        Failed = 0,
        Success,
    }

    //区域提交是否成功标识
    enum RegionResponse : byte
    {
        Failed = 0,
        Success,
    }

    //图像representation枚举
    enum ImageRepresentation : ushort
    {
        UINT8 = 0,
        MinUnsigned = UINT8,
        SINT8,
        MinSinged = SINT8,
        UINT16,
        SINT16,
        UINT32,
        MaxUnsigned = UINT32,
        SINT32,
        MaxSigned = SINT32,
    }

    //图像压缩类型枚举
    public enum ImageCompressType
    {
        Compress_Unknown = 0,
        Compress_NoCompres,//1
        Compress_JPEG_LOSSLESS,
        Compress_JPEG_LOS,
        Compress_RLE,
        Compress_JPEG2000,//5
    }

    //后台仿真消息ID枚举
    enum SimulateMsg
    {
        BuildExam = 10,
    }

    class PacketDelimiter
    {
        public static readonly byte[] PacketBegin = { 0x0B, 0x0D, 0x0B, 0x0D };
        public static readonly byte[] PacketEnd = { 0x1C, 0x0D, 0x1C, 0x0D };
    }

    //用来分隔检查ID和区域标识的连接符
    class RegionDelimiter
    {
        public static string Connector = "#";
        public static string Tidy(string value)
        {
            string[] splitor = { Connector };
            string[] ret = value.Split(splitor, StringSplitOptions.None);
            return ret[0];
        }
    }
    public class UpdateResponseStruct
    {
        public string id;
        public bool bValid;
        public string strErr;
    }
    public class ExtendPrintRequest
    {
        public string examId = "";
        public string seriesId = "";
        public string imageId = "";
        public byte[] data = null;
    }

    //终端类型
    enum ClientType
    {
        Desktop, //桌面版
        Web,     //网页版
        Mobile,   //移动端
        Ris,     //Ris终端
    }
}
