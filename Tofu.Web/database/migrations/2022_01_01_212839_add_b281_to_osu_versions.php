<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class AddB281ToOsuVersions extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        DB::table("osu_versions")
			->insert(
				array
				(
					"version" => "b282",
					"version_handler" => "b282",
					"allow_bancho" => true,
					"allow_scores" => true,
					"hash" => "cant verify on b282",
				)
			);
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::table('osu_versions', function (Blueprint $table) {
            //
        });
    }
}
