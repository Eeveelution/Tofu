<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateBanchoVersionTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('osu_versions', function (Blueprint $table) {
            $table->id();
            $table->string("version", 16)->nullable(false)->index();
            $table->string("version_handler", 16)->nullable(false)->index();
            $table->boolean("allow_bancho")->default(true)->nullable(false);
            $table->boolean("allow_scores")->default(true)->nullable(false);
            $table->string("hash")->nullable(false)->unique()->index();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('osu_versions');
    }
}
