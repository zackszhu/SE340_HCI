# zzs模型文件归约

## Header

        struct header {
            int point;    //2D/3D模型的点的个数（两个模型应该有一样的点数）
            int 3dface;   //3D模型的面的个数
        };
        
----

## 3D Model Description

        struct 3DModelPoint {
            double x, y, z; //点的三维空间坐标
            int mapping;    //点对应的二维空间坐标
        };
        struct 3DModelFace {
            // 不选择将一些大的面拆成几个小的三角形面片，不易于贴图
            int n;  //点的个数
            int[n] p;   //面上的每个点的序号（顺序链接）
            int dir     //映射到纸的正面还是反面
            int color;  //RGB888
        };
        
----

## 2D Model Description
        
        struct 2DModelPoint {
            double x, y; //点的二维坐标
        };
        
